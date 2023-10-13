namespace ARI.Models;

public record ArmResult<T>(
    [property: JsonPropertyName("value")]
    T[] Value,
    [property: JsonPropertyName("nextLink")]
    Uri NextLink
);