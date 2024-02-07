using ARI.Models.Tenant.Subscription.ResourceGroup.Resource;

namespace ARI.Tests.Unit.Extensions;

[TestFixture]
public class TextWriterMarkdownExtensionsTests
{
    [TestCase(false, false, false, false)]
    [TestCase(true, false, false, false)]
    [TestCase(false, true, false, false)]
    [TestCase(false, false, true, false)]
    [TestCase(false, false, false, true)]
    public async Task AddFrontmatter(
        bool skipFrontmatter,
        bool skipFrontmatterSummary,
        bool skipFrontmatterModified,
        bool skipFrontmatterOrder
        )
    {
        // Given
        var sw = new StringWriter();
        var lastUpdateTime = DateTimeOffset.MaxValue;
        var summary = "Summary";
        var order = 0;
        var settings = new ARI.Commands.Settings.InventorySettings
        { 
            SkipFrontmatter = skipFrontmatter,
            SkipFrontmatterSummary = skipFrontmatterSummary,
            SkipFrontmatterModified = skipFrontmatterModified,
            SkipFrontmatterOrder = skipFrontmatterOrder
        };

        // When
        await sw.AddFrontmatter(
            lastUpdateTime,
            summary,
            order,
            settings
            );

        // Then
        await Verify(sw);
    }

    [Test]
    public async Task AddTenantOverview()
    {
        // Given
        var sw = new StringWriter();
        var tenant = MocksFixture.AzureTenant;

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
        var subscription = MocksFixture.Subscription;

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
        var tags = MocksFixture.Tags;

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
        var resourceGroup = MocksFixture.ResourceGroup;

        // When
        await sw.AddResourceGroupOverview(
            resourceGroup
            );

        // Then
        await Verify(sw);
    }

    [Test]
    public async Task AddChildrenIndex()
    {
        // Given
        var sw = new StringWriter();
        var children = MocksFixture.Children;

        // When
        await sw.AddChildrenIndex(
            children
            );

        // Then
        await Verify(sw);
    }

    [Test]
    public async Task AddResourceOverview()
    {
        // Given
        var sw = new StringWriter();
        var resource = MocksFixture.Resource;

        // When
        await sw.AddResourceOverview(
            resource
            );

        // Then
        await Verify(sw);
    }

    [TestCase("2023-10-15 23:08+0")]
    public async Task AddNameDescriptionRow(DateTimeOffset? description)
    {
        // Given
        var sw = new StringWriter();

        // When
        await sw.AddNameDescriptionRow(
            description
            );

        // Then
        await Verify(sw);
    }

    [TestCase(0)]
    public async Task AddNameDescriptionRow(int? description)
    {
        // Given
        var sw = new StringWriter();

        // When
        await sw.AddNameDescriptionRow(
            description
            );

        // Then
        await Verify(sw);
    }

    [TestCase("string")]
    [TestCase(null)]
    public async Task AddNameDescriptionRow(string? description)
    {
        // Given
        var sw = new StringWriter();

        // When
        await sw.AddNameDescriptionRow(
            description
            );

        // Then
        await Verify(sw);
    }

    [TestCase()]
    [TestCase("requestTracingEnabled")]
    public async Task AddProperties(params string[] allowedSiteProperties)
    {
        // Given
        var sw = new StringWriter();
        var properties = MocksFixture.Properties;
        var settings = allowedSiteProperties.Any()
            ? new ARI.Commands.Settings.InventorySettings
            {
                AllowedSiteProperties = allowedSiteProperties
            }
            : new ARI.Commands.Settings.InventorySettings();
        
        // When
        await sw.AddProperties(properties, settings);

        // Then
        await Verify(sw);
    }



    [TestCase(false, true)]
    [TestCase(false, true, "00000000-0000-0000-0000-000000000000")]
    [TestCase(false, false, "APPINSIGHTS_INSTRUMENTATIONKEY")]
    [TestCase(true, true)]
    [TestCase(true, true, "00000000-0000-0000-0000-000000000000")]
    [TestCase(true, false, "APPINSIGHTS_INSTRUMENTATIONKEY")]
    public async Task AddSettings(bool includeSiteApplicationsettings, bool isValue, params string[] allowedSiteSettingValues)
    {
        // Given
        var sw = new StringWriter();
        var Settings = MocksFixture.Settings;
        var settings = allowedSiteSettingValues.Any()
            ? isValue
                ? new ARI.Commands.Settings.InventorySettings
                    {
                        IncludeSiteApplicationsettings = includeSiteApplicationsettings,
                        AllowedSiteSettingValues = allowedSiteSettingValues
                    }
                : new ARI.Commands.Settings.InventorySettings
                    {
                        IncludeSiteApplicationsettings = includeSiteApplicationsettings,
                        AllowedSiteSettingValueKeys = allowedSiteSettingValues
                    }
            : new ARI.Commands.Settings.InventorySettings
                {
                    IncludeSiteApplicationsettings = includeSiteApplicationsettings
            };

        // When
        await sw.AddSettings(Settings, settings);

        // Then
        await Verify(sw);
    }

    [TestCase()]
    [TestCase("../../")]
    public async Task AddResourcesIndex(string? path = null)
    {
        // Given
        var sw = new StringWriter();
        var resources = new IResource[]
        {
            MocksFixture.ResourceGroup,
            MocksFixture.Resource
        };

        // When
        await sw.AddResourcesIndex(resources, path);


        // Then
        await Verify(sw);
    }
}
