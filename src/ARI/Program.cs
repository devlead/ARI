using ARI.Commands;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Configuration;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Azure.Core;
using Azure.Identity;
using ARI.Services.ARM;

var serviceCollection = new ServiceCollection()
    .AddCakeCore()
    .AddLogging(configure =>
            configure
                .AddSimpleConsole(opts =>
                {
                    opts.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
                })
                .AddConfiguration(
                new ConfigurationBuilder()
                    .Add(new MemoryConfigurationSource
                    {
                        InitialData = new Dictionary<string, string?>
                        {
                            { "LogLevel:System.Net.Http.HttpClient", "Warning" }
                        }
                    })
                    .Build()
            ))
    .AddSingleton<AzureTokenService>(
        async (tenantId, scope) =>
        {
            var tokenCredential = new DefaultAzureCredential();
            var accessToken = await tokenCredential.GetTokenAsync(
                new TokenRequestContext(
                    tenantId: tenantId,
                    scopes: new string[] {
                        scope ?? "https://management.azure.com/.default"
                    }
                    )
            );
            return accessToken;
        }
    )
    .AddSingleton<InventoryCommand>()
    .AddSingleton<TokenService>()
    .AddSingleton<TenantService>()
    .AddSingleton<SubscriptionService>();

serviceCollection.AddHttpClient();

using var registrar = new DependencyInjectionRegistrar(serviceCollection);
var app = new CommandApp(registrar);

app.Configure(
    config =>
    {
        config.ValidateExamples();

        config.AddCommand<InventoryCommand>("inventory")
                .WithDescription("Example inventory command.")
                .WithExample(new[] { "inventory", "00000000-0000-0000-0000-000000000000", "outputpath" });
    });

return await app.RunAsync(args);