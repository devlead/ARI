namespace ARI.Models.Tenant;

public record AzureResourceTag(
        string Tag,
        ILookup<string, (IResource Resource, DirectoryPath? Path)> Resources
        ) : AzureResourceBase, IResource
{
    public override string PublicId { get; } = Tag;

    public override string Description => $"Resources tag {Tag}";
    string IResource.Id => PublicId;
    string IResource.Location => "Global";
    string IResource.Type => nameof(ResourceGroup);
}