using ARI.Models.Tenant.Subscription.ResourceGroup;
using ARI.Models.Tenant.Subscription.ResourceGroup.Resource;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

namespace ARI.Extensions;

public static class TextWriterMarkdownExtensions
{
    public const int NameColumnWidth = 35;
    public const int TypeColumnWidth = 55;
    public const int DescriptionColumnWidth = 95;

    public static async Task AddFrontmatter(
        this TextWriter writer,
        DateTimeOffset lastUpdateTime,
        string summary,
        int order)
    {
        await writer.WriteLineAsync(
            FormattableString.Invariant(
                    $$"""
                    ---
                    summary: {{summary}}
                    modifiedby: ARI
                    modified: {{lastUpdateTime:yyyy-MM-dd HH:mm}}
                    order: {{order}}
                    ---
                    """
                )
            );
    }

    public static async Task AddTenantOverview(
        this TextWriter writer,
        AzureTenant tenant)
    {
        await writer.WriteLineAsync(
            FormattableString.Invariant(
                    $$"""
                    # Overview
                   
                    |                                     |                                                                                                 |
                    |-------------------------------------|-------------------------------------------------------------------------------------------------|
                    | **Name**                            | {{tenant.DisplayName.CodeLine(),-DescriptionColumnWidth}} |
                    | **Id**                              | {{tenant.Id.CodeLine(),-DescriptionColumnWidth}} |
                    | **Tenant Id**                       | {{tenant.TenantId.CodeLine(),-DescriptionColumnWidth}} |
                    | **Country code**                    | {{tenant.CountryCode.CodeLine(),-DescriptionColumnWidth}} |
                    | **Default domain**                  | {{tenant.DefaultDomain.CodeLine(),-DescriptionColumnWidth}} |
                    | **Domains**                         | {{string.Join(Environment.NewLine, tenant.Domains).CodeLine(),-DescriptionColumnWidth}} |
                    """
                )
            );
    }

    public static async Task AddSubscriptionOverview(
        this TextWriter writer,
        Subscription subscription)
    {
        await writer.WriteLineAsync(
            FormattableString.Invariant(
                    $$"""
                    # Overview
                   
                    |                                     |                                                                                                 |
                    |-------------------------------------|-------------------------------------------------------------------------------------------------|
                    | **Name**                            | {{subscription.DisplayName.CodeLine(),-DescriptionColumnWidth}} |
                    | **Id**                              | {{subscription.Id.CodeLine(),-DescriptionColumnWidth}} |
                    | **Subscription Id**                 | {{subscription.SubscriptionId.CodeLine(),-DescriptionColumnWidth}} |
                    | **State**                           | {{subscription.State.CodeLine(),-DescriptionColumnWidth}} |
                    """
                )
            );
    }

    public static async Task AddResourceGroupOverview(
        this TextWriter writer,
        ResourceGroup resourceGroup
        )
    {
        await writer.WriteLineAsync(
            FormattableString.Invariant(
                    $$"""
                    # Overview
                   
                    |                                     |                                                                                                 |
                    |-------------------------------------|-------------------------------------------------------------------------------------------------|
                    | **Name**                            | {{resourceGroup.Name.CodeLine(),-DescriptionColumnWidth}} |
                    | **Id**                              | {{resourceGroup.Id.CodeLine(),-DescriptionColumnWidth}} |
                    | **Location**                        | {{resourceGroup.Location.CodeLine(),-DescriptionColumnWidth}} |
                    """
                )
            );
    }

    public static async Task AddTags(
        this TextWriter writer,
        IDictionary<string, string> tags
        )
    {
        await writer.WriteLineAsync(
           FormattableString.Invariant(
                   $$"""

                   ## Tags
                   
                   | Tag                                 | Value                                                                                           |
                   |-------------------------------------|-------------------------------------------------------------------------------------------------|
                   """
               )
           );

        if (tags == null)
        {
            return;
        }

        foreach (var (key, value) in tags)
        {
            await writer.WriteLineAsync(
                FormattableString.Invariant(
                    $"| {key.Bold(),-NameColumnWidth} | {value.CodeLine(),-DescriptionColumnWidth} |"
                )
            );
        }
    }

    public static async Task AddChildrenIndex(
        this TextWriter writer,
        IEnumerable<AzureResourceBase> children,
        string? headline = null,
        int level = 2,
        [CallerArgumentExpression(nameof(children))]
        string headlineFallback = ""
        )
    {
        await writer.WriteLineAsync(
           FormattableString.Invariant(
                   $$"""

                   {{string.Empty.PadLeft(level, '#')}} {{headline ?? CultureInfo.InvariantCulture.TextInfo.ToTitleCase(headlineFallback)}}
                   
                   |                                                                                                 |                                                                                                 |
                   |-------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------|
                   """
               )
           );

        foreach (var (key, value) in children)
        {
            await writer.WriteLineAsync(
                FormattableString.Invariant(
                    $"| {key.Link(),-DescriptionColumnWidth} | {value.Link(key),-DescriptionColumnWidth} |"
                )
            );
        }
    }

    public static async Task AddResourceOverview(
        this TextWriter writer,
        Resource resource
        )
    {
        await writer.WriteLineAsync(
            FormattableString.Invariant(
                    $$"""
                    # Overview
                   
                    |                                     |                                                                                                 |
                    |-------------------------------------|-------------------------------------------------------------------------------------------------|
                    | **Name**                            | {{resource.Name.CodeLine(),-DescriptionColumnWidth}} |
                    | **Id**                              | {{resource.Id.CodeLine(),-DescriptionColumnWidth}} |
                    | **Location**                        | {{resource.Location.CodeLine(),-DescriptionColumnWidth}} |
                    | **ProvisioningState**               | {{resource.ProvisioningState.CodeLine(),-DescriptionColumnWidth}} |
                    | **CreatedTime**                     | {{resource.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).CodeLine(),-DescriptionColumnWidth}} |
                    | **ChangedTime**                     | {{resource.ChangedTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).CodeLine(),-DescriptionColumnWidth}} |
                    | **Type**                            | {{resource.Type.CodeLine(),-DescriptionColumnWidth}} |
                    """
                )
            );

        if (resource.SKU != null)
        {
            await writer.WriteLineAsync(
                FormattableString.Invariant(
                        $$"""

                        ## SKU
                        
                        |                                     |                                                                                                 |
                        |-------------------------------------|-------------------------------------------------------------------------------------------------|
                        """
                    )
            );
            await writer.AddNameDescriptionRow(resource.SKU.Capacity);
            await writer.AddNameDescriptionRow(resource.SKU.Family);
            await writer.AddNameDescriptionRow(resource.SKU.Model);
            await writer.AddNameDescriptionRow(resource.SKU.Name);
            await writer.AddNameDescriptionRow(resource.SKU.Size);
            await writer.AddNameDescriptionRow(resource.SKU.Tier);

        }
    }

    public static Task AddNameDescriptionRow(
        this TextWriter writer,
        DateTimeOffset? description,
        [CallerArgumentExpression(nameof(description))]
        string name = ""
    )
        => writer.AddNameDescriptionRow(
            description?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            name
            );

    public static Task AddNameDescriptionRow(
        this TextWriter writer,
        int? description,
        [CallerArgumentExpression(nameof(description))]
        string name = ""
    )
        => writer.AddNameDescriptionRow(
            description?.ToString(CultureInfo.InvariantCulture),
            name
            );

    public static Task AddNameDescriptionRow(
            this TextWriter writer,
            string? description,
            [CallerArgumentExpression(nameof(description))]
            string name = ""
        )
        => string.IsNullOrWhiteSpace(description)
        ? Task.CompletedTask
        : writer.WriteLineAsync(
                FormattableString.Invariant(
                    $"| {name.LastPart('.').Bold(),-NameColumnWidth} | {description.CodeLine(),-DescriptionColumnWidth} |"
                )
            );
}
