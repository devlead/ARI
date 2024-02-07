using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;

namespace ARI.Models.Tenant;

public abstract record AzureResourceBase
{
    public abstract string PublicId { get; }
    public abstract string Description { get; }

    [JsonPropertyName("tenantId")]
    public string TenantId { get; init; } = string.Empty;

    [JsonPropertyName("subscriptionId")]
    public string SubscriptionId { get; init; } = string.Empty;

    public string ResourceGroupName { get; init; } = string.Empty;
    public int Order { get; init; } = 0;
    
    [JsonPropertyName("tags")]
    public AzureResourceTags Tags { get; init; } = AzureResourceTags.Empty;

    public virtual void Deconstruct(out string key, out string value)
    {
        key = PublicId;
        value = Description;
    }
}
