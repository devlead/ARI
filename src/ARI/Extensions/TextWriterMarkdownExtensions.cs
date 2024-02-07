using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace ARI.Extensions;

public static class TextWriterMarkdownExtensions
{
    public const int NameColumnWidth = 35;
    public const int TypeColumnWidth = 55;
    public const int DescriptionColumnWidth = 95;
    public const int SettingKeyColumnWidth = 60;
    public const int SettingValueColumnWidth = 120;

    public static async Task AddFrontmatter(
        this TextWriter writer,
        DateTimeOffset lastUpdateTime,
        string summary,
        int order,
        InventorySettings settings
        )
    {
        if (settings.SkipFrontmatter)
        {
            return;
        }

        await writer.WriteLineAsync("---");

        if (!settings.SkipFrontmatterSummary)
        {
            await writer.WriteLineAsync(FormattableString.Invariant($"summary: {summary}"));
        }

        await writer.WriteLineAsync(FormattableString.Invariant($"modifiedby: ARI"));

        if (!settings.SkipFrontmatterModified)
        {
            await writer.WriteLineAsync(FormattableString.Invariant($"modified: {lastUpdateTime:yyyy-MM-dd HH:mm}"));
        }

        if (!settings.SkipFrontmatterOrder)
        {
            await writer.WriteLineAsync(FormattableString.Invariant($"order: {order}"));
        }

        await writer.WriteLineAsync("---");
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

    private static readonly JsonSerializerOptions PropertiesValueOptions = new() { WriteIndented = true };
    public static async Task AddProperties(
        this TextWriter writer,
        IDictionary<string, JsonValue> properties,
        InventorySettings settings
        )
    {
        await writer.WriteLineAsync(
           FormattableString.Invariant(
                   $$"""

                   ## Properties
                   
                   | Key                                 | Value                                                                                           |
                   |-------------------------------------|-------------------------------------------------------------------------------------------------|
                   """
               )
           );

        if (properties == null)
        {
            return;
        }

        foreach (var (key, value) in properties)
        {
            if (!settings.AllowedSiteProperties.Contains(key))
            {
                settings.SkippedSiteProperties.Add(key);
                continue;
            }

            await writer.WriteLineAsync(
                FormattableString.Invariant(
                    $"| {key.SeparateByCase().Bold(),-NameColumnWidth} | {value?.ToJsonString(PropertiesValueOptions).PreLine(),-DescriptionColumnWidth} |"
                )
            );
        }
    }

    public static async Task AddSettings(
        this TextWriter writer,
        IDictionary<string, string> properties,
        InventorySettings settings
        )
    {
        if (!settings.IncludeSiteApplicationsettings)
        {
            return;
        }

        await writer.WriteLineAsync(
           FormattableString.Invariant(
                   $$"""

                   ## Settings
                   
                   | Key                                                          | Value                                                                                                                    |
                   |--------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------|
                   """
               )
           );

        if (properties == null)
        {
            return;
        }

        foreach (var (key, value) in properties)
        {
            var displayValue = string.IsNullOrWhiteSpace(value)
                ? string.Empty
                :   settings.AllowedSiteSettingValues.Contains(value) 
                        ? value
                        : "*".PadRight((value.Length > SettingValueColumnWidth ? SettingValueColumnWidth : value.Length) / 2, '*');


            await writer.WriteLineAsync(
                FormattableString.Invariant(
                    $"| {key.Bold(),-SettingKeyColumnWidth} | {displayValue?.PreLine(),-SettingValueColumnWidth} |"
                )
            );
        }
    }

    public static async Task AddResourcesIndex(
        this TextWriter writer,
        IEnumerable<IResource> resources,
        DirectoryPath? path = null
        )
        => await writer.AddResourcesIndex(
            resources.Select(
                resoruce => (resoruce, default(DirectoryPath))
                ),
            path
            );

    public static async Task AddResourcesIndex(
        this TextWriter writer,
        IEnumerable<(IResource Resource, DirectoryPath? Path)> resources,
        DirectoryPath? path = null
        )
    {
        var resourcesByTypeLookup = resources.ToLookup(
                               key => (key.Resource.Type.Split('/', count: 2, options: StringSplitOptions.TrimEntries) is string[] { Length: > 0 } group)
                                       ? group[0]
                                       : key.Resource.Type,
                               value => (
                               SubType: (value.Resource.Type.Split('/', count: 2, options: StringSplitOptions.TrimEntries) is string[] { Length: > 1 } group)
                                       ? group[1]
                                       : value.Resource.Type,
                               Resource: value.Resource,
                               Path: value.Path
                               ),
                               StringComparer.OrdinalIgnoreCase
                           );

        await writer.WriteLineAsync(
                """

                ## Resources

                | | | |
                |-|-|-|
                """
            );

        foreach (var resourcesByType in resourcesByTypeLookup.OrderBy(key => key.Key, StringComparer.OrdinalIgnoreCase))
        {
            await writer.WriteLineAsync($"| **{resourcesByType.Key}** | | |");

            foreach (var (subType, resource, resourcePath) in resourcesByType.OrderBy(key => key.Resource.Description, StringComparer.OrdinalIgnoreCase))
            {
                var href = path == null
                    ? resource.PublicId
                    : resourcePath != null
                        ? path.GetRelativePath(resourcePath).FullPath
                        :   path
                                .CombineWithFilePath(resource.PublicId)
                                .FullPath;

                await writer.WriteLineAsync($"| {resource.Description.Link(href)} | {subType.Link(href)} | {resource.Location.Link(href)} |");
            }

            await writer.WriteLineAsync("| | |");
        }
    }
}
