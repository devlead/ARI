using ARI.Models.Tenant;
using ARI.Models.Tenant.Subscription;
using ARI.Models.Tenant.Subscription.ResourceGroup;

namespace ARI.Tests.Unit.Extensions;

[TestFixture]
public class TextWriterMarkdownExtensionsTests
{
    [Test]
    public async Task AddFrontmatter()
    {
        // Given
        var sw = new StringWriter();
        var lastUpdateTime = DateTimeOffset.MaxValue;
        var summary = "Summary";
        var order = 0;

        // When
        await sw.AddFrontmatter(
            lastUpdateTime,
            summary,
            order
            );

        // Then
        await Verify(sw);
    }

    [Test]
    public async Task AddTenantOverview()
    {
        // Given
        var sw = new StringWriter();
        var tenant = new AzureTenant(
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

        // When
        await sw.AddTenantOverview(
            tenant
            );

        // Then
        await Verify(sw);
    }

    [Test]
    public async Task AddSubscriptionOverview()
    {
        // Given
        var sw = new StringWriter();
        var subscription = new Subscription(
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

        // When
        await sw.AddSubscriptionOverview(
            subscription
            );

        // Then
        await Verify(sw);
    }

    [Test]
    public async Task AddTags()
    {
        // Given
        var sw = new StringWriter();
        var tags = new Dictionary<string, string>
                                {
                                    { "Tag1", "Value1" },
                                    { "Tag2", "Value2" }
                                };

        // When
        await sw.AddTags(tags);

        // Then
        await Verify(sw);
    }

    [Test]
    public async Task AddResourceGroupOverview()
    {
        // Given
        var sw = new StringWriter();
        var resourceGroup = new ResourceGroup(
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

        // When
        await sw.AddResourceGroupOverview(
            resourceGroup
            );

        // Then
        await Verify(sw);
    }
}
