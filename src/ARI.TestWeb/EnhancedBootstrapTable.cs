using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Extensions.Tables;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace BRI.TestWeb;

public sealed class EnhancedBootstrapTable : IMarkdownExtension
{
    /// <inheritdoc />
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        // Make sure we don't have a delegate twice
        pipeline.DocumentProcessed -= PipelineOnDocumentProcessed;
        pipeline.DocumentProcessed += PipelineOnDocumentProcessed;
    }

    /// <inheritdoc />
    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
    }

    /// <inheritdoc />
    private void PipelineOnDocumentProcessed(MarkdownDocument document)
    {
        if (document is null || document.Parser is null)
        {
            return;
        }

        var descendants = document.ToArray();
        

        for (var i = 0; i < descendants.Length; i++)
        {
            if (descendants[i] is not Table table)
            {
                continue;
            }

            document.Remove(table);

            table.GetAttributes().AddClass("table-striped boxed");

            var container = new CustomContainer(document.Parser);
            container.GetAttributes().AddClass("table-responsive");
            container.Add(table);

            document.Insert(i, container);
        }
    }
}