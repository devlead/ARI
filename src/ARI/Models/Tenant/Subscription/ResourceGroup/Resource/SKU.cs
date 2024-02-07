namespace ARI.Models.Tenant.Subscription.ResourceGroup.Resource;

public record SKU(
    [property: JsonPropertyName("capacity")]
    int? Capacity,
    [property: JsonPropertyName("family")]
    string? Family,
    [property: JsonPropertyName("model")]
    string? Model,
    [property: JsonPropertyName("name")]
    string? Name,
    [property: JsonPropertyName("size")]
    string? Size,
    [property: JsonPropertyName("tier")]
    string? Tier
    );