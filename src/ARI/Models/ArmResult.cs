namespace ARI.Models;

public record ArmResult<T>(
    [property: JsonPropertyName("value")]
    T[] Value
);