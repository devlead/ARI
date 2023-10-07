namespace ARI.Models.Graph;

public record GraphOrgDomain(
    [property: JsonPropertyName("isDefault")]
    bool IsDefault,
    [property: JsonPropertyName("name")]
    string Name
    );
