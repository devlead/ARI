namespace ARI.Models.Tenant.Subscription.ResourceGroup.Resource.Web.Site;

public record WebSettings(
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("type")]
    string Type,
    [property: JsonPropertyName("location")]
    string Location,
    [property: JsonPropertyName("properties")]
    SortedDictionary<string, string> Properties
    ) : AzureResourceBase
{
    public override string PublicId => Id;

    public override string Description => Name;
}