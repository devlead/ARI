namespace ARI.Models.Tenant.Subscription;

public record Subscription(
    [property:JsonPropertyName("id")]
    string Id,
    [property:JsonPropertyName("subscriptionId")]
    string SubscriptionId,
    [property:JsonPropertyName("tenantId")]
    string TenantId,
    [property:JsonPropertyName("displayName")]
    string DisplayName,
    [property:JsonPropertyName("state")]
    string State,
    [property:JsonPropertyName("subscriptionPolicies")]
    SubscriptionPolicies SubscriptionPolicies,
    [property:JsonPropertyName("authorizationSource")]
    string AuthorizationSource,
    [property:JsonPropertyName("managedByTenants")]
    ManagedByTenant[] ManagedByTenants
) : AzureResourceBase
{
    public override string Description => DisplayName;
}
