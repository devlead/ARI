using ARI.Models.Tenant;
using ARI.Models.Tenant.Subscription;
using ARI.Models.Tenant.Subscription.ResourceGroup;

namespace ARI.Tests.Fixture;
public static class MocksFixture
{
    public static AzureTenant AzureTenant { get; } = new (
                                                        "Id",
                                                        "TenantId",
                                                        "CountryCode",
                                                        "DisplayName",
                                                        new[] { "Domain1", "Domain2" },
                                                        "TenantCategory",
                                                        "DefaultDomain",
                                                        "TenantType",
                                                        "TenantBrandingLogoUrl"
                                                    );
    public static Subscription Subscription { get; } = new(
                                                            "Id",
                                                            "SubscriptionId",
                                                            "TenantId",
                                                            "DisplayName",
                                                            "State",
                                                            new SubscriptionPolicies(
                                                                    "LocationPlacementId",
                                                                    "QuotaId",
                                                                    "SpendingLimit"
                                                                ),
                                                            "AuthorizationSource",
                                                            Array.Empty<ManagedByTenant>()
                                                        );
    public static IDictionary<string, string> Tags { get; } = new Dictionary<string, string>
                                {
                                    { "Tag1", "Value1" },
                                    { "Tag2", "Value2" }
                                }.AsReadOnly();

    public static ResourceGroup ResourceGroup { get; } = new(
                                                                "Id",
                                                                "Location",
                                                                "ManagedBy",
                                                                "Name",
                                                                new Dictionary<string, string>
                                                                {
                                                                    { "provisioningState", "Succeeded" }
                                                                },
                                                                "Type"
                                                            );
    public static ICollection<AzureResourceBase> Children { get; } = new AzureResourceBase[]
                                                                        {
                                                                            AzureTenant,
                                                                            Subscription,
                                                                            ResourceGroup,
                                                                        };
}
