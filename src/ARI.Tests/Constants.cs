namespace ARI.Tests;

public class Constants
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
                        public const string Settings = $"https://management.azure.com/subscriptions/{Tenant.Subscription1.Id}/resourceGroups/{Tenant.Subscription1.ResourceGroup1.Name}/providers/Microsoft.Web/sites/{Tenant.Subscription1.ResourceGroup1.Site.Name}/config/appsettings/list?api-version=2022-03-01";
                    }
                }
                public static class ResourceGroup2
                {
                    public const string Resources = $"https://management.azure.com/subscriptions/{Tenant.Subscription1.Id}/resourceGroups/{Tenant.Subscription1.ResourceGroup2.Name}/resources?$expand=createdTime,changedTime,provisioningState&api-version=2021-04-01";
                    public static class Site
                    {
                        public const string Config = $"https://management.azure.com/subscriptions/{Tenant.Subscription1.Id}/resourceGroups/{Tenant.Subscription1.ResourceGroup2.Name}/providers/Microsoft.Web/sites/{Tenant.Subscription1.ResourceGroup2.Site.Name}/config?api-version=2022-03-01";
                        public const string Settings = $"https://management.azure.com/subscriptions/{Tenant.Subscription1.Id}/resourceGroups/{Tenant.Subscription1.ResourceGroup2.Name}/providers/Microsoft.Web/sites/{Tenant.Subscription1.ResourceGroup2.Site.Name}/config/appsettings/list?api-version=2022-03-01";
                    }
                }
                public static class ResourceGroup3
                {
                    public const string Resources = $"https://management.azure.com/subscriptions/{Tenant.Subscription1.Id}/resourceGroups/{Tenant.Subscription1.ResourceGroup3.Name}/resources?$expand=createdTime,changedTime,provisioningState&api-version=2021-04-01";
                    public static class Site
                    {
                        public const string Config = $"https://management.azure.com/subscriptions/{Tenant.Subscription1.Id}/resourceGroups/{Tenant.Subscription1.ResourceGroup3.Name}/providers/Microsoft.Web/sites/{Tenant.Subscription1.ResourceGroup3.Site.Name}/config?api-version=2022-03-01";
                        public const string Settings = $"https://management.azure.com/subscriptions/{Tenant.Subscription1.Id}/resourceGroups/{Tenant.Subscription1.ResourceGroup3.Name}/providers/Microsoft.Web/sites/{Tenant.Subscription1.ResourceGroup3.Site.Name}/config/appsettings/list?api-version=2022-03-01";
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
}
