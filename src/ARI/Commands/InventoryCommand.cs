using Cake.Common.IO;

namespace ARI.Commands;

public class InventoryCommand : AsyncCommand<InventorySettings>
{
    private ICakeContext CakeContext { get; }
    private ILogger Logger { get; }
    private TenantService TenantService { get; }
    private SubscriptionService SubscriptionService { get; }
    private ResourceGroupService ResourceGroupService { get; }

    public override async Task<int> ExecuteAsync(CommandContext context, InventorySettings settings)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var modified = DateTimeOffset.UtcNow;
        Logger.LogInformation("TenantId: {TenantId}", settings.TenantId);
        Logger.LogInformation("OutputPath: {OutputPath}", settings.OutputPath);

        var tenant = await TenantService.GetTenant(settings.TenantId);

        var targetPath = settings.OutputPath.Combine(tenant.DefaultDomain);

        Logger.LogInformation("Cleaning directory {TargetPath}...", targetPath);
        CakeContext.CleanDirectory(targetPath);
        Logger.LogInformation("Done cleaning directory {TargetPath}.", targetPath);

        using var writer = CakeContext
                          .OpenIndexWrite(targetPath);

        await writer.AddFrontmatter(
            modified,
            $"Tenant {tenant.DisplayName} ({tenant.TenantId})",
            1
            );

        await writer.AddTenantOverview(tenant);

        Logger.LogInformation("Getting subscriptions...");
        var subscriptions = await SubscriptionService.GetSubscriptions(settings.TenantId);
        Logger.LogInformation("Found {SubscriptionCount}", subscriptions.Count);
        await Parallel.ForEachAsync(
            subscriptions,
            async (subscription, ct) =>
            {
                var subscriptionPath = targetPath.Combine(subscription.SubscriptionId);

                using var writer = CakeContext
                                    .OpenIndexWrite(subscriptionPath);

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

                await Parallel.ForEachAsync(
                    resourceGroups,
                    async (resourceGroup, ct) =>
                    {
                        var resourceGroupPath = targetPath
                                                    .Combine(subscription.SubscriptionId)
                                                    .Combine(resourceGroup.Name);

                        using var writer = CakeContext
                                            .OpenIndexWrite(resourceGroupPath);

                        await writer.AddFrontmatter(
                            modified,
                            $"Resource {resourceGroup.Name} ({subscription.SubscriptionId})",
                            resourceGroup.Order
                            );

                        await writer.AddResourceGroupOverview(resourceGroup);

                        await writer.AddTags(resourceGroup.Tags);
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
        ResourceGroupService resourceGroupService
        )
    {
        CakeContext = cakeContext;
        Logger = logger;
        TenantService = tenantService;
        SubscriptionService = subscriptionService;
        ResourceGroupService = resourceGroupService;
    }
}
