using ARI.Models.Tenant;
using ARI.Models.Tenant.Subscription;
using ARI.Models.Tenant.Subscription.ResourceGroup;
using Spectre.Console.Cli;

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
}
