using ARI.Commands.Settings;
using ARI.Commands;
using Cake.Core;
using Spectre.Console.Cli;

namespace ARI.Tests.Unit.Commands;

[TestFixture]
public class InventoryCommandTests
{
    [Test]
    public async Task ExecuteAsync()
    {
        // Given
        var context = new CommandContext(
            Substitute.For<IRemainingArguments>(),
            nameof(InventoryCommand),
            null
            );
        var settings = new InventorySettings
        {
            TenantId = Constants.Tenant.Id,
            OutputPath = "/home/docs"
        };
        var (
            cakeContext,
            logger,
            tenantService,
            subscriptionService
            ) = ARIServiceProviderFixture.GetRequiredService<ICakeContext, ILogger<InventoryCommand>, TenantService, SubscriptionService>(
                services => services
                                .AddCakeFakes(
                                    fileSystem => fileSystem.CreateDirectory(settings.OutputPath)
                                )
            );

        var command = new InventoryCommand(
            cakeContext,
            logger,
            tenantService,
            subscriptionService
            );

        // When
        var result = new
        {
            ExitCode = await command.ExecuteAsync(
                            context,
                            settings
                        ),
            Output = cakeContext.FileSystem.FromDirectoryPath(settings.OutputPath)
        };

        // Then
        await Verify(result);
    }
}

