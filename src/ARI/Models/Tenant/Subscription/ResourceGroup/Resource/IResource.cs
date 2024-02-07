namespace ARI.Models.Tenant.Subscription.ResourceGroup.Resource;

public interface IResource
{
    string Id { get; }
    string PublicId { get; }
    string Location { get; }
    string Description { get; }
    string Type { get; }
    AzureResourceTags Tags { get; }
}