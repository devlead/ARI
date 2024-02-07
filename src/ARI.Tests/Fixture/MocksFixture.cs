using ARI.Models.Tenant;
using ARI.Models.Tenant.Subscription;
using ARI.Models.Tenant.Subscription.ResourceGroup;
using ARI.Models.Tenant.Subscription.ResourceGroup.Resource;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ARI.Tests.Fixture;
public static class MocksFixture
{
    public static AzureTenant AzureTenant { get; } = new AzureTenant(
                                                        "Id",
                                                        "CountryCode",
                                                        "DisplayName",
                                                        new[] { "Domain1", "Domain2" },
                                                        "TenantCategory",
                                                        "DefaultDomain",
                                                        "TenantType",
                                                        "TenantBrandingLogoUrl"
                                                    ) with {
                                                        TenantId = "TenantId"
                                                    };
    public static Subscription Subscription { get; } = new Subscription(
                                                            "Id",
                                                            "DisplayName",
                                                            "State",
                                                            new SubscriptionPolicies(
                                                                    "LocationPlacementId",
                                                                    "QuotaId",
                                                                    "SpendingLimit"
                                                                ),
                                                            "AuthorizationSource",
                                                            Array.Empty<ManagedByTenant>()
                                                        ) with
                                                        {
                                                            TenantId = "TenantId",
                                                            SubscriptionId = "SubscriptionId"
                                                        };
    public static IDictionary<string, string> Tags { get; } = new AzureResourceTags
                                {
                                    { "Tag3", "Value4" },
                                    { "Tag1", "Value1" },
                                    { "Tag2", "Value2" },
                                    { "Tag0", "Value3" }
                                }.AsReadOnly();

    public static ResourceGroup ResourceGroup { get; } = new ResourceGroup(
                                                                "Id",
                                                                "Location",
                                                                "ManagedBy",
                                                                "Name",
                                                                new AzureResourceProperties
                                                                {
                                                                    { "provisioningState", "Succeeded" }
                                                                },
                                                                "Microsoft.Resources/resourceGroups"
                                                            ) with
                                                            {
                                                                TenantId = "TenantId",
                                                                SubscriptionId = "SubscriptionId"
                                                            };
    public static ICollection<AzureResourceBase> Children { get; } = new AzureResourceBase[]
                                                                        {
                                                                            AzureTenant,
                                                                            Subscription,
                                                                            ResourceGroup,
                                                                        };
    public static Resource Resource { get; } = new Resource(
                                                "Id",
                                                "Location",
                                                "ManagedBy",
                                                "Name",
                                                DateTimeOffset.MinValue,
                                                DateTimeOffset.MaxValue,
                                                new AzureResourceProperties
                                                {
                                                    {"Property1", "Value1" },
                                                    {"Property0", "Value2" }
                                                },
                                                "ProvisioningState",
                                                "Type",
                                                new SKU(
                                                    0,
                                                    "Family",
                                                    "Model",
                                                    "Name",
                                                    "Size",
                                                    "Tier"
                                                    )
                                                );
#pragma warning disable CS8601 // Possible null reference assignment.
    public static IDictionary<string, JsonValue> Properties { get; } = JsonSerializer.Deserialize<SortedDictionary<string, JsonValue>>(
        """
        {
            "alwaysOn": false,
            "http20Enabled": false,
            "loadBalancing": "LeastRequests",
            "minTlsVersion": "1.2",
            "numberOfWorkers": 1,
            "use32BitWorkerProcess": true,
            "vnetName": "",
            "webSocketsEnabled": false,
            "requestTracingEnabled": false,
            "remoteDebuggingEnabled": false,
            "remoteDebuggingVersion": "VS2019",
            "httpLoggingEnabled": false,
            "azureMonitorLogCategories": null,
            "acrUseManagedIdentityCreds": false,
            "acrUserManagedIdentityID": null,
            "logsDirectorySizeLimit": 35,
            "detailedErrorLoggingEnabled": false,
            "publishingUsername": "$lab-web-web-dev",
            "defaultDocuments": [
                "Default.htm",
                "Default.html",
                "Default.asp",
                "index.htm",
                "index.html",
                "iisstart.htm",
                "default.aspx",
                "index.php",
                "hostingstart.html"
            ],
            "experiments": {
                "rampUpRules": []
            }
        }
        """
        );
#pragma warning restore CS8601 // Possible null reference assignment.

    public static IDictionary<string, string> Settings { get; } = new SortedDictionary<string, string>
    {
        { "APPINSIGHTS_INSTRUMENTATIONKEY", "00000000-0000-0000-0000-000000000000"},
        { "APPLICATIONINSIGHTS_CONNECTION_STRING", "InstrumentationKey=00000000-0000-0000-0000-000000000000;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/"},
        { "WEBSITE_RUN_FROM_PACKAGE", "1"},
        { "SECRET_GREETING", "@Microsoft.KeyVault(SecretUri=https://lab-kv-prd.vault.azure.net/secrets/secret-greeting/)" }
    };
}
