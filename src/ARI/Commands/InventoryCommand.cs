using ARI.Extensions;
using Cake.Common.IO;

namespace ARI.Commands;

public class InventoryCommand : AsyncCommand<InventorySettings>
{
    private ICakeContext CakeContext { get; }
    private ILogger Logger { get; }
    private TenantService TenantService { get; }
    private SubscriptionService SubscriptionService { get; }
    private ResourceGroupService ResourceGroupService { get; }
    private ResourceService ResourceService { get; }

    public override async Task<int> ExecuteAsync(CommandContext context, InventorySettings settings)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var modified = DateTimeOffset.UtcNow;
        Logger.LogInformation("TenantId: {TenantId}", settings.TenantId);
        Logger.LogInformation("OutputPath: {OutputPath}", settings.OutputPath);

        var tenant = await TenantService.GetTenant(settings.TenantId);

        var targetPath = (!settings.SkipTenantOverview)
                            ? settings.OutputPath.Combine(tenant.DefaultDomain)
                            : settings.OutputPath;

        Logger.LogInformation("Cleaning directory {TargetPath}...", targetPath);
        CakeContext.CleanDirectory(targetPath);
        Logger.LogInformation("Done cleaning directory {TargetPath}.", targetPath);

        using var writer = CakeContext
                          .OpenIndexWrite(targetPath);

        if (!settings.SkipTenantOverview)
        {
            await writer.AddFrontmatter(
                modified,
                $"Tenant {tenant.DisplayName} ({tenant.TenantId})",
                1
            );

            await writer.AddTenantOverview(tenant);
        }
        else
        {
            await writer.AddFrontmatter(
                modified,
                "Azure Inventory",
                1
            );
        }

        Logger.LogInformation("Getting subscriptions...");
        var subscriptions = await SubscriptionService.GetSubscriptions(settings.TenantId);
        Logger.LogInformation("Found {SubscriptionCount}", subscriptions.Count);

        await writer.AddChildrenIndex(subscriptions);

        await Parallel.ForEachAsync(
            subscriptions,
            async (subscription, ct) =>
            {
                using var writer = CakeContext
                                    .OpenIndexWrite(targetPath, subscription, out var subscriptionPath);

                await writer.AddFrontmatter(
                    modified,
                    $"Subscription {subscription.DisplayName} ({subscription.TenantId})",
                    subscription.Order
                    );

                await writer.AddSubscriptionOverview(subscription);

                await writer.AddTags(subscription.Tags);

                var resourceGroups = await ResourceGroupService.GetResourceGroups(
                    subscription.TenantId,
                    subscription.SubscriptionId
                    );

                await writer.AddChildrenIndex(resourceGroups);

                await Parallel.ForEachAsync(
                    resourceGroups,
                    async (resourceGroup, ct) =>
                    {
                        using var writer = CakeContext
                                            .OpenIndexWrite(subscriptionPath, resourceGroup, out var resourceGroupPath);

                        await writer.AddFrontmatter(
                            modified,
                            $"Resource Group {resourceGroup.Name} ({subscription.SubscriptionId})",
                            resourceGroup.Order
                            );

                        await writer.AddResourceGroupOverview(resourceGroup);

                        await writer.AddTags(resourceGroup.Tags);

                        var resources = await ResourceService.GetResources(
                            settings.TenantId,
                            subscription.SubscriptionId,
                            resourceGroup.Name
                            );

                        var resourcesByTypeLookup = resources.ToLookup(
                                key => (key.Type.Split('/', count:2, options: StringSplitOptions.TrimEntries) is string[] { Length:>0} group)
                                        ? group[0]
                                        : key.Type,
                                value => (
                                SubType: (value.Type.Split('/', count: 2, options: StringSplitOptions.TrimEntries) is string[] { Length: > 1 } group)
                                        ? group[1]
                                        : value.Type,
                                Resource: value
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

                            foreach(var (subType, resource) in resourcesByType.OrderBy(key => key.Resource.Description, StringComparer.OrdinalIgnoreCase))
                            {
                                await writer.WriteLineAsync($"| {resource.Description.Link(resource.PublicId)} | {subType.Link(resource.PublicId)} | {resource.Location.Link(resource.PublicId)} |");
                            }

                            await writer.WriteLineAsync("| | |");
                        }


                        await Parallel.ForEachAsync(
                            resources,
                            async (resource, ct) =>
                            {
                                using var writer = CakeContext
                                            .OpenIndexWrite(resourceGroupPath, resource, out var resourcePath);

                                await writer.AddFrontmatter(
                                    modified,
                                    $"Resource {resource.Name} ({subscription.SubscriptionId})",
                                    resource.Order
                                    );

                                await writer.AddResourceOverview(resource);

                                await writer.AddTags(resource.Tags);
                            }
                        );
                    }
                );
            }
        );

        sw.Stop();
        Logger.LogInformation("Processed {SubscriptionCount} in {Elapsed}", subscriptions.Count, sw.Elapsed);

        return 0;
    }

    public InventoryCommand(
        ICakeContext cakeContext,
        ILogger<InventoryCommand> logger,
        TenantService tenantService,
        SubscriptionService subscriptionService,
        ResourceGroupService resourceGroupService,
        ResourceService resourceService
        )
    {
        CakeContext = cakeContext;
        Logger = logger;
        TenantService = tenantService;
        SubscriptionService = subscriptionService;
        ResourceGroupService = resourceGroupService;
        ResourceService = resourceService;
    }
}
