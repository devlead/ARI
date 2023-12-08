using static System.Net.WebRequestMethods;

namespace ARI.Tests;

public static class Constants
{
    public static class Tenant
    {
        public const string Id = "daea2e9b-847b-4c93-850d-2aa6f2d7af33";
        public static class Subscription1
        {
            public const string Id = "291bba3f-e0a5-47bc-a099-3bdcb2a50a05";
            public static class ResourceGroup1
            {
                public const string Name = "lab-dev";
                public static class Site
                {
                    public const string Name = "lab-web-web-dev";
                }
            }
            public static class ResourceGroup2
            {
                public const string Name = "lab-stg";
                public static class Site
                {
                    public const string Name = "lab-web-web-stg";
                }
            }
            public static class ResourceGroup3
            {
                public const string Name = "lab-prd";
                public static class Site
                {
                    public const string Name = "lab-web-web-prd";
                }
            }
        }
        public static class Subscription2
        {
            public const string Id = "72ac930a-f34e-42d8-b06d-dc2a9e12ed71";
            public static class ResourceGroup1
            {
                public const string Name = "common-dev";
            }
        }
    }

    public static class Request
    {
        public static class Method
        {
            public const string
                Post = "POST",
                Get = "GET";
        }

        public static class Uri
        {
            public const string
                Tenants = "https://management.azure.com/tenants?api-version=2020-01-01",
                Subscriptions = "https://management.azure.com/subscriptions?api-version=2020-01-01",
                GraphOrg = $"https://graph.microsoft.com/beta/organization/{Tenant.Id}";

            public static class Subscription1
            {
                public const string ResourceGroups = $"https://management.azure.com/subscriptions/{Tenant.Subscription1.Id}/resourcegroups?api-version=2021-04-01";
                public static class ResourceGroup1
                {
                    public const string Resources = $"https://management.azure.com/subscriptions/{Tenant.Subscription1.Id}/resourceGroups/{Tenant.Subscription1.ResourceGroup1.Name}/resources?$expand=createdTime,changedTime,provisioningState&api-version=2021-04-01";
                    public static class Site
                    {
                        public const string Config = $"https://management.azure.com/subscriptions/{Tenant.Subscription1.Id}/resourceGroups/{Tenant.Subscription1.ResourceGroup1.Name}/providers/Microsoft.Web/sites/{Tenant.Subscription1.ResourceGroup1.Site.Name}/config?api-version=2022-03-01";
                    }
                }
                public static class ResourceGroup2
                {
                    public const string Resources = $"https://management.azure.com/subscriptions/{Tenant.Subscription1.Id}/resourceGroups/{Tenant.Subscription1.ResourceGroup2.Name}/resources?$expand=createdTime,changedTime,provisioningState&api-version=2021-04-01";
                    public static class Site
                    {
                        public const string Config = $"https://management.azure.com/subscriptions/{Tenant.Subscription1.Id}/resourceGroups/{Tenant.Subscription1.ResourceGroup2.Name}/providers/Microsoft.Web/sites/{Tenant.Subscription1.ResourceGroup2.Site.Name}/config?api-version=2022-03-01";
                    }
                }
                public static class ResourceGroup3
                {
                    public const string Resources = $"https://management.azure.com/subscriptions/{Tenant.Subscription1.Id}/resourceGroups/{Tenant.Subscription1.ResourceGroup3.Name}/resources?$expand=createdTime,changedTime,provisioningState&api-version=2021-04-01";
                    public static class Site
                    {
                        public const string Config = $"https://management.azure.com/subscriptions/{Tenant.Subscription1.Id}/resourceGroups/{Tenant.Subscription1.ResourceGroup3.Name}/providers/Microsoft.Web/sites/{Tenant.Subscription1.ResourceGroup3.Site.Name}/config?api-version=2022-03-01";
                    }
                }
            }

            public static class Subscription2
            {
                public const string ResourceGroups = $"https://management.azure.com/subscriptions/{Tenant.Subscription2.Id}/resourcegroups?api-version=2021-04-01";
                public static class ResourceGroup1
                {
                    public const string Resources = $"https://management.azure.com/subscriptions/{Tenant.Subscription2.Id}/resourceGroups/{Tenant.Subscription2.ResourceGroup1.Name}/resources?$expand=createdTime,changedTime,provisioningState&api-version=2021-04-01";
                }
            }
        }
    }

    public static class Response
    {
        public static class Json
        {
            public static string TenantsServicePrinciple { get; } = GetResourceString($"{nameof(TenantsServicePrinciple)}.json");
            public static string TenantsUser { get; } = GetResourceString($"{nameof(TenantsUser)}.json");
            public static string GraphOrg { get; } = GetResourceString($"{nameof(GraphOrg)}.json");
            public static string Subscriptions { get; } = GetResourceString($"{nameof(Subscriptions)}.json");

            public static class Subscription1
            {
                public static string ResourceGroups { get; } = GetResourceString($"{nameof(Subscription1)}_{nameof(ResourceGroups)}.json");
                public static class ResourceGroup1
                {
                    public static string Resources { get; } = GetResourceString($"{nameof(Subscription1)}_{nameof(ResourceGroup1)}_{nameof(Resources)}.json");
                    public static class Site
                    {
                        public static string Config { get; } = GetResourceString($"{nameof(Subscription1)}_{nameof(ResourceGroup1)}_{nameof(Site)}_{nameof(Config)}.json");
                    }
                }
                public static class ResourceGroup2
                {
                    public static string Resources { get; } = GetResourceString($"{nameof(Subscription1)}_{nameof(ResourceGroup2)}_{nameof(Resources)}.json");
                    public static class Site
                    {
                        public static string Config { get; } = GetResourceString($"{nameof(Subscription1)}_{nameof(ResourceGroup2)}_{nameof(Site)}_{nameof(Config)}.json");
                    }
                }
                public static class ResourceGroup3
                {
                    public static string Resources { get; } = GetResourceString($"{nameof(Subscription1)}_{nameof(ResourceGroup3)}_{nameof(Resources)}.json");
                    public static class Site
                    {
                        public static string Config { get; } = GetResourceString($"{nameof(Subscription1)}_{nameof(ResourceGroup3)}_{nameof(Site)}_{nameof(Config)}.json");
                    }
                }
            }

            public static class Subscription2
            {
                public static string ResourceGroups { get; } = GetResourceString($"{nameof(Subscription2)}_{nameof(ResourceGroups)}.json");
                public static class ResourceGroup1
                {
                    public static string Resources { get; } = GetResourceString($"{nameof(Subscription2)}_{nameof(ResourceGroup1)}_{nameof(Resources)}.json");
                }
            }
        }
    }

    public static class MediaType
    {
        public const string
            Json = "application/json";
    }

    private static string GetResourceString(string filename, Encoding? encoding = null)
    {
        using var stream = GetResourceStream(filename);
        using var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);
        return reader.ReadToEnd();
    }

    private static Stream GetResourceStream(string filename)
    {
        var resourceStream = typeof(ARIServiceProviderFixture)
                                .Assembly
                                .GetManifestResourceStream($"ARI.Tests.Resources.{filename}");

        return resourceStream
            ?? throw new Exception($"Failed to get stream for {filename}.");
    }
}
