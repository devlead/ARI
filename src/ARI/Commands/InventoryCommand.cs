using ARI.Extensions;
using ARI.Services.ARM;
using Cake.Common.IO;

namespace ARI.Commands;

public class InventoryCommand : AsyncCommand<InventorySettings>
{
    private ICakeContext CakeContext { get; }
    private ILogger Logger { get; }
    private TenantService TenantService { get; }
    private SubscriptionService SubscriptionService { get; }

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

        using var stream = CakeContext
                          .FileSystem
                          .GetFile(targetPath.CombineWithFilePath("index.md"))
                          .OpenWrite();

        using var writer = new StreamWriter(
            stream,
            System.Text.Encoding.UTF8
        );

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

                CakeContext.EnsureDirectoryExists(subscriptionPath);

                using var stream = CakeContext
                          .FileSystem
                          .GetFile(subscriptionPath.CombineWithFilePath("index.md"))
                          .OpenWrite();

                using var writer = new StreamWriter(
                    stream,
                    Encoding.UTF8
                );

                await writer.AddFrontmatter(
                    modified,
                    $"Subscription {subscription.DisplayName} ({subscription.TenantId})",
                    subscription.Order
                    );

                await writer.AddSubscriptionOverview(subscription);
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
        SubscriptionService subscriptionService
        )
    {
        CakeContext = cakeContext;
        Logger = logger;
        TenantService = tenantService;
        SubscriptionService = subscriptionService;
    }
}
