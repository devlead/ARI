using ARI.Extensions;
using ARI.Models.Tenant.Subscription.ResourceGroup.Resource;

namespace ARI.Tests.Unit.Extensions;

[TestFixture]
public class StringMarkdownExtensionsTests
{
    [TestCase("CodeLine(\r\n\"value\"\r\n)")]
    public async Task CodeLine(string value)
    {
        // Given / When
        var result = value.CodeLine();

        // Then
        await Verify(result);
    }


    [TestCase("PreLine(\r\n\"value\"\r\n)")]
    public async Task PreLine(string value)
    {
        // Given / When
        var result = value.PreLine();

        // Then
        await Verify(result);
    }

    [TestCase("SingleLine(\r\n\"value\"\r\n)")]
    public async Task SingleLine(string value)
    {
        // Given / When
        var result = value.SingleLine();

        // Then
        await Verify(result);
    }

    [TestCase("Bold")]
    public async Task Bold(string value)
    {
        // Given / When
        var result = value.Bold();

        // Then
        await Verify(result);
    }

    [TestCase("Description", null)]
    [TestCase("Description", "Href")]
    public async Task Link(string description, string? href)
    {
        // Given / When
        var result = description.Link(href);

        // Then
        await Verify(result);
    }

    [TestCase("Resource")]
    [TestCase("Resource.SKU")]
    [TestCase("Resource.SKU.Tier")]
    public async Task LastPart(string value)
    {
        // Given / When
        var result = value.LastPart('.');

        // Then
        await Verify(result);
    }
}
