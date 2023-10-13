using Newtonsoft.Json.Linq;
using System.Collections.Immutable;

namespace ARI.Models.Tenant;

public abstract record AzureResourceBase
{
    public static Dictionary<string, string> Empty { get; } = new Dictionary<string, string>();
    public abstract string Description { get; }
    public int Order { get; init; } = 0;
    
    [JsonPropertyName("tags")]
    public Dictionary<string, string> Tags { get; init; } = Empty;
}
