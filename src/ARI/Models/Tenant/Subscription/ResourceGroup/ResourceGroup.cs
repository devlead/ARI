namespace ARI.Models.Tenant.Subscription.ResourceGroup;

public record ResourceGroup(
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonPropertyName("location")]
    string Location,
    [property: JsonPropertyName("managedBy")]
    string ManagedBy,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("properties")]
    Dictionary<string,string> Properties,
    [property: JsonPropertyName("type")]
    string Type
    ) : AzureResourceBase
{
    public override string PublicId => Name;
    public override string Description => Name;
    public override void Deconstruct(out string key, out string value)
    {
        key = Name;
        value = Location;
    }
}
