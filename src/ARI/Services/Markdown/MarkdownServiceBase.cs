namespace ARI.Services.Markdown;
public abstract record MarkdownServiceBase(
    string Type
    )
{
    public abstract Task Render(
        TextWriter writer,
        Resource resource,
        InventorySettings settings
        );
}