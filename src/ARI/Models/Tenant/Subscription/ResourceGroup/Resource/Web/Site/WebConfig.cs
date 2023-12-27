using System.Text.Json.Nodes;

namespace ARI.Models.Tenant.Subscription.ResourceGroup.Resource.Web.Site;

public record WebConfig(
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("type")]
    string Type,
    [property: JsonPropertyName("location")]
    string Location,
    [property: JsonPropertyName("properties")]
    SortedDictionary<string, JsonValue> Properties
    ) : AzureResourceBase
{
    public override string PublicId => Id;

    public override string Description => Name;
}
