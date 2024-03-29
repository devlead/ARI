﻿namespace ARI.Models.Tenant.Subscription.ResourceGroup.Resource;

public record Resource(
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonPropertyName("location")]
    string Location,
    [property: JsonPropertyName("managedBy")]
    string ManagedBy,
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("createdTime")]
    DateTimeOffset CreatedTime,
    [property: JsonPropertyName("changedTime")]
    DateTimeOffset ChangedTime,
    [property: JsonPropertyName("properties")]
    AzureResourceProperties Properties,
    [property: JsonPropertyName("provisioningState")]
    string ProvisioningState,
    [property: JsonPropertyName("type")]
    string Type,
    [property: JsonPropertyName("sku")]
    SKU? SKU
    ) : AzureResourceBase, IResource
{
    public override string PublicId => Name;
    public override string Description => Name;
    public override void Deconstruct(out string key, out string value)
    {
        key = Name;
        value = Location;
    }
}
