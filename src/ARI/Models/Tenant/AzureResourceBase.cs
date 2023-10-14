using Newtonsoft.Json.Linq;
using System.Collections.Immutable;

namespace ARI.Models.Tenant;

public abstract record AzureResourceBase
{
    private static Dictionary<string, string> Empty { get; } = new Dictionary<string, string>();

    public abstract string PublicId { get; }
    public abstract string Description { get; }
    
    public int Order { get; init; } = 0;
    
    [JsonPropertyName("tags")]
    public Dictionary<string, string> Tags { get; init; } = Empty;

    public virtual void Deconstruct(out string key, out string value)
    {
        key = PublicId;
        value = Description;
    }
}
