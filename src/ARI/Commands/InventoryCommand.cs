using Cake.Common.IO;
using Microsoft.Identity.Client;
using System.Collections.Concurrent;

namespace ARI.Commands;

public class InventoryCommand : AsyncCommand<InventorySettings>
{
    private ICakeContext CakeContext { get; }
    private ILogger Logger { get; }
    private TenantService TenantService { get; }
    private SubscriptionService SubscriptionService { get; }
    private ResourceGroupService ResourceGroupService { get; }
    private ResourceService ResourceService { get; }
    public ILookup<string, MarkdownServiceBase> MarkdownServices { get; }
    private static AzureResourceTags HasTags { get; } = new AzureResourceTags
    {
        { "Has-tags", "true" }
    };
    private static AzureResourceTags NoTags { get; } = new AzureResourceTags
    {
        { "Has-tags", "false" }
    };

    public override async Task<int> ExecuteAsync(CommandContext context, InventorySettings settings)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var modified = DateTimeOffset.UtcNow;
        var markDownFileName = settings.MarkdownName + ".md";
        var resourcesBag = new ConcurrentBag<(IResource Resource, DirectoryPath? Path)>();
        if (settings.OutputPath.IsRelative)
        {
            Logger.LogInformation("Relative outputpath {outputpath} making absolute...", settings.OutputPath);
            settings.OutputPath = settings.OutputPath.MakeAbsolute(CakeContext.Environment);
        }
        Logger.LogInformation("TenantId: {TenantId}", settings.TenantId);
        Logger.LogInformation("OutputPath: {OutputPath}", settings.OutputPath);
        Logger.LogInformation("Generate report in parallel: {GenerateInParallel}", settings.SkipTenantOverview);
        Logger.LogInformation("Using markdown filename: {MarkDownFileName}", markDownFileName);

        var tenant = await TenantService.GetTenant(settings.TenantId);

        var targetPath = (!settings.SkipTenantOverview)
                            ? settings.OutputPath.Combine(tenant.DefaultDomain)
                            : settings.OutputPath;
        Logger.LogInformation("Cleaning directory {TargetPath}...", targetPath);
        CakeContext.CleanDirectory(targetPath);
        Logger.LogInformation("Done cleaning directory {TargetPath}.", targetPath);

        using var writer = CakeContext
                          .OpenIndexWrite(targetPath, markDownFileName);

        if (!settings.SkipTenantOverview)
        {
            await writer.AddFrontmatter(
                modified,
                $"Tenant {tenant.DisplayName} ({tenant.TenantId})",
                1,
                settings
            );

            await writer.AddTenantOverview(tenant);
        }
        else
        {
            await writer.AddFrontmatter(
                modified,
                "Azure Inventory",
                1,
                settings
            );
        }

        Logger.LogInformation("Getting subscriptions...");
        var subscriptions = await SubscriptionService.GetSubscriptions(settings.TenantId);
        Logger.LogInformation("Found {SubscriptionCount}", subscriptions.Count);

        await writer.AddChildrenIndex(subscriptions);

        await ForEachAsync(
            settings,
            subscriptions,
            async (subscription, ct) =>
            {
                using var writer = CakeContext
                                    .OpenIndexWrite(targetPath, subscription, markDownFileName, out var subscriptionPath);

                resourcesBag.Add((subscription, subscriptionPath));

                await writer.AddFrontmatter(
                    modified,
                    $"Subscription {subscription.DisplayName} ({subscription.TenantId})",
                    subscription.Order,
                    settings
                    );

                await writer.AddSubscriptionOverview(subscription);

                await writer.AddTags(subscription.Tags);

                var resourceGroups = await ResourceGroupService.GetResourceGroups(
                    subscription.TenantId,
                    subscription.SubscriptionId
                    );

                await writer.AddChildrenIndex(resourceGroups);

                await ForEachAsync(
                    settings,
                    resourceGroups,
                    async (resourceGroup, ct) =>
                    {
                        using var writer = CakeContext
                                            .OpenIndexWrite(subscriptionPath, resourceGroup, markDownFileName, out var resourceGroupPath);

                        resourcesBag.Add((resourceGroup, resourceGroupPath));

                        await writer.AddFrontmatter(
                            modified,
                            $"Resource Group {resourceGroup.Name} ({subscription.SubscriptionId})",
                            resourceGroup.Order,
                            settings
                            );

                        await writer.AddResourceGroupOverview(resourceGroup);

                        await writer.AddTags(resourceGroup.Tags);

                        var resources = await ResourceService.GetResources(
                            settings.TenantId,
                            subscription.SubscriptionId,
                            resourceGroup.Name
                            );

                        await writer.AddResourcesIndex(resources);

                        await ForEachAsync(
                            settings,
                            resources,
                            async (resource, ct) =>
                            {
                                using var writer = CakeContext
                                            .OpenIndexWrite(resourceGroupPath, resource, markDownFileName, out var resourcePath);

                                resourcesBag.Add((resource, resourcePath));

                                await writer.AddFrontmatter(
                                    modified,
                                    $"Resource {resource.Name} ({subscription.SubscriptionId})",
                                    resource.Order,
                                    settings
                                    );

                                await writer.AddResourceOverview(resource);

                                await writer.AddTags(resource.Tags);

                                foreach (var renderer in MarkdownServices[resource.Type])
                                {
                                    await renderer.Render(writer, resource, settings);
                                }
                            }
                        );
                    }
                );
            }
        );

        var tags = (
                from resource in resourcesBag
                from tag in resource.Resource.Tags.Concat(resource.Resource.Tags.Count == 0 ? NoTags : HasTags)
                group (tag, resource) by tag.Key into taggroup
                select (
                    Tag: taggroup.Key.ToLowerInvariant(),
                    Resources: taggroup.ToArray()
                )
            )
            .ToLookup(
                key => key.Tag.ToLowerInvariant(),
                value => value,
                StringComparer.OrdinalIgnoreCase
            )
            .Select(
                tags=> new AzureResourceTag(
                    tags.Key,
                    tags
                    .SelectMany(value=>value.Resources)
                    .ToLookup(
                        key => key.tag.Value,
                        value => value.resource,
                        StringComparer.OrdinalIgnoreCase
                        )
                    )
            )
            .IndexResources(
                tenantIdFunc => tenant.TenantId
            )
            .ToArray();


        await writer.AddChildrenIndex(tags);
        await ForEachAsync(
            settings,
            tags,
            async (tag, ct) =>
            {
                //var tagPath = tagsPath.Combine(tag.Tag);
                using var writer = CakeContext
                                   .OpenIndexWrite(targetPath, tag, markDownFileName, out var tagPath);

                await writer.AddFrontmatter(
                    modified,
                    $"Resources for tag {tag.Tag}",
                    tag.Order,
                    settings
                    );

                await writer.WriteLineAsync(
                       FormattableString.Invariant(
                               $$"""

                                ## Values
                        
                                |                                                                                                 |
                                |-------------------------------------------------------------------------------------------------|
                                """
                           )
                   );
                var order = 0;
                foreach (var resources in tag.Resources)
                {
                    await writer.WriteLineAsync($"| {resources.Key.Link()} |");

                    using var valueWriter = CakeContext
                                                .OpenIndexWrite(tagPath, resources.Key, markDownFileName, out var tagValuePath);

                    await valueWriter.AddFrontmatter(
                        modified,
                        $"{tag.PublicId} = {resources.Key}",
                        Interlocked.Increment(ref order),
                        settings
                        );

                    await valueWriter.AddResourcesIndex(resources, tagValuePath);
                }
            }
            );

        sw.Stop();
        Logger.LogInformation("Processed {SubscriptionCount} in {Elapsed}", subscriptions.Count, sw.Elapsed);

        if (settings.OutputSkippedSiteProperties && settings.SkippedSiteProperties.Any())
        {
            Logger.LogInformation(
                $"Skipped site properties:{Environment.NewLine}{{SkippedSiteProperties}}",
                string.Join(
                    Environment.NewLine,
                    settings.SkippedSiteProperties
                )
                );
        }

        return 0;
    }

    async Task ForEachAsync<TSource>(
        InventorySettings settings,
        IEnumerable<TSource> source, 
        Func<TSource, CancellationToken, ValueTask> body)
    {
        if (settings.RunInParallel)
        {
            await Parallel.ForEachAsync(source, body);
        }
        else
        {
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;
            foreach (var v in source)
            {
                await body(v, ct);
                if (ct.IsCancellationRequested)
                {
                    return;
                }
            }

            return;
        }
        
    }
    public InventoryCommand(
        ICakeContext cakeContext,
        ILogger<InventoryCommand> logger,
        TenantService tenantService,
        SubscriptionService subscriptionService,
        ResourceGroupService resourceGroupService,
        ResourceService resourceService,
        IEnumerable<MarkdownServiceBase> markdownServices
        )
    {
        CakeContext = cakeContext;
        Logger = logger;
        TenantService = tenantService;
        SubscriptionService = subscriptionService;
        ResourceGroupService = resourceGroupService;
        ResourceService = resourceService;
        MarkdownServices = markdownServices.ToLookup(
            key => key.Type,
            value => value,
            StringComparer.OrdinalIgnoreCase
            );
    }
}
