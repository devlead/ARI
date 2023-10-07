namespace ARI.Models.Tenant.Subscription;

public record SubscriptionPolicies(
   [property:JsonPropertyName("locationPlacementId")]
   string LocationPlacementId,
   [property:JsonPropertyName("quotaId")]
   string QuotaId,
   [property:JsonPropertyName("spendingLimit")]
   string SpendingLimit
);
