namespace ARI.Tests;

public static class Constants
{
    public static class Tenant
    {
        public const string Id = "daea2e9b-847b-4c93-850d-2aa6f2d7af33";
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
