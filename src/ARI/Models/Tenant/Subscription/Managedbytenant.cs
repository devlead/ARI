namespace ARI.Models.Tenant.Subscription;

public record ManagedByTenant(
   [property:JsonPropertyName("tenantId")]
   string TenantId
);
