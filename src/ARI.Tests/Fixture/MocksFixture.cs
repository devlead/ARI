using ARI.Models.Tenant;
using ARI.Models.Tenant.Subscription;
using ARI.Models.Tenant.Subscription.ResourceGroup;
using ARI.Models.Tenant.Subscription.ResourceGroup.Resource;
using System.Reflection;

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
    public static IDictionary<string, string> Tags { get; } = new AzureResourceTags
                                {
                                    { "Tag3", "Value4" },
                                    { "Tag1", "Value1" },
                                    { "Tag2", "Value2" },
                                    { "Tag0", "Value3" }
                                }.AsReadOnly();

    public static ResourceGroup ResourceGroup { get; } = new(
                                                                "Id",
                                                                "Location",
                                                                "ManagedBy",
                                                                "Name",
                                                                new AzureResourceProperties
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
    public static Resource Resource { get; } = new Resource(
                                                "Id",
                                                "Location",
                                                "ManagedBy",
                                                "Name",
                                                DateTimeOffset.MinValue,
                                                DateTimeOffset.MaxValue,
                                                new AzureResourceProperties
                                                {
                                                    {"Property1", "Value1" },
                                                    {"Property0", "Value2" }
                                                },
                                                "ProvisioningState",
                                                "Type",
                                                new SKU(
                                                    0,
                                                    "Family",
                                                    "Model",
                                                    "Name",
                                                    "Size",
                                                    "Tier"
                                                    )
                                                );
}
