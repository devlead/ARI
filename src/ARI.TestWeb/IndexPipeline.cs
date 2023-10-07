using Statiq.Web.Modules;
using Statiq.Web.Pipelines;
using System.Text;

namespace BRI.TestWeb;


/// <summary>
/// Pipeline for creating index documents.
/// </summary>
public sealed class Index : Content
{
    /// <inheritdoc />
    public Index(Templates templates) : base(templates)
    {
        Dependencies.AddRange(new[]
        {
            nameof(Content),
            nameof(Archives),
            nameof(DirectoryMetadata)
        });


        InputModules = new ModuleList
        {

        };

        ProcessModules = new ModuleList
        {
             new ConcatDocuments(
                nameof(Content),
                nameof(Archives)),
            new CreateTree()
                .WithNesting()
                .WithPlaceholderFactory(TreePlaceholderFactory),
            new FlattenTree(),
            new FilterDocuments(Config.FromDocument(x => x.GetBool(Keys.TreePlaceholder))),
            new SetMediaType(MediaTypes.Markdown),
                new CacheDocuments
                {
                    new AddTitle(),
                    new SetDestination(true),
                    new ExecuteIf(Config.FromSetting(WebKeys.OptimizeContentFileNames, true))
                    {
                        new OptimizeFileName()
                    },
                    new RenderContentProcessTemplates(templates),
                    new ExecuteIf(Config.FromDocument(doc => doc.MediaTypeEquals(MediaTypes.Html)))
                    {
                        // Excerpts and headings only work for HTML content
                        new GenerateExcerpt(),
                        new GatherHeadings(Config.FromDocument(WebKeys.GatherHeadingsLevel, 2))
                    }
                },
                new OrderDocuments()
        };
    }

    private static Task<IDocument> TreePlaceholderFactory(object[] path, IMetadata metadata, IExecutionContext context)
    {
        NormalizedPath linkRoot = context.Settings.GetPath(Keys.LinkRoot, "/");

        var indexPath = new NormalizedPath(string.Join("/", path.Concat(new[] { "index.html" })));
        var inputPath = context.FileSystem
            .RootPath.Combine(context.FileSystem.InputPaths[0])
            .Combine(indexPath);

        // Create a Markdown-based index placeholder
        var builder = new StringBuilder();
        builder.AppendLine($"# {path.LastOrDefault()}");
        builder.AppendLine();

        var children = metadata.GetDocumentList(Keys.Children)
            .OrderByDescending(x => x.GetBool(Keys.TreePlaceholder))
            .ThenBy(x => x.Get(Keys.Order, default(long)))
            .ThenBy(x => x.GetString(Keys.Title, x.Destination.Parent.Name));

        builder.AppendLine("| Documents | Summary |");
        builder.AppendLine("| --------- | ------- |");

        foreach (var child in children)
        {
            var name = child.GetString(Keys.Title, child.Destination.Parent.Name);
#pragma warning disable SYSLIB0013
            var link = string.Concat(
                linkRoot.IsNullOrEmpty ? string.Empty : "/",
                Uri.EscapeUriString(linkRoot.FullPath ?? string.Empty),
                "/",
                Uri.EscapeUriString(child.Destination.FullPath ?? string.Empty)
                );
#pragma warning restore SYSLIB0013
            var summary = child.GetString("Summary", child.GetString("Description"));
            builder.AppendLine($"| [{name}]({link}) | {summary} |");
        }

        string fullPath = string.Join("/", path);
        return Task.FromResult(
            context.CreateDocument(inputPath, indexPath, new MetadataDictionary(metadata)
            {
                [Keys.Title] = fullPath,
                ["Index"] = true,
                ["Summary"] = string.Concat("Index for ", fullPath),
                ["ShowInNavbar"] = true,
                ["IsPage"] = true,
                ["Tags"] = new[]{ "TOC" }
            }, context.GetContentProvider(builder.ToString()))
        );
    }
}