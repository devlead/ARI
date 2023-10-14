using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using VerifyTests.Http;

namespace ARI.Tests.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCakeFakes(
        this IServiceCollection services,
        Action<FakeFileSystem>? configureFileSystem = null
        )
    {
        var configuration = new FakeConfiguration();
        
        var environment = FakeEnvironment.CreateUnixEnvironment();
        
        var fileSystem = new FakeFileSystem(environment);
        configureFileSystem?.Invoke(fileSystem);

        var globber = new Globber(fileSystem, environment);
        
        var log = new FakeLog();

        var Context = Substitute.For<ICakeContext>();
        Context.Configuration.Returns(configuration);
        Context.Environment.Returns(environment);
        Context.FileSystem.Returns(fileSystem);
        Context.Globber.Returns(globber);
        Context.Log.Returns(log);

        return services.AddSingleton<ICakeConfiguration>(configuration)
                                .AddSingleton<ICakeEnvironment>(environment)
                                .AddSingleton<FakeFileSystem>(fileSystem)
                                .AddSingleton<IFileSystem>(fileSystem)
                                .AddSingleton<IGlobber>(globber)
                                .AddSingleton<ICakeLog>(log)
                                .AddSingleton<ICakeRuntime>(environment.Runtime)
                                .AddSingleton<ICakeContext>(Context);
    }

    public static IServiceCollection AddMockHttpClient(this IServiceCollection services)
    {
        static HttpResponseMessage GetMockTenantsResponse()
        => new()
        {
            Content = new StringContent(
                                    Constants.Response.Json.TenantsServicePrinciple,
                                    Encoding.UTF8,
                                    Constants.MediaType.Json
                                )
        };
        static HttpResponseMessage GetMockGraphOrgResponse()
        => new()
        {
            Content = new StringContent(
                                    Constants.Response.Json.GraphOrg,
                                    Encoding.UTF8,
                                    Constants.MediaType.Json
                                )
        };
        static HttpResponseMessage GetMockSubscriptionsResponse()
        => new()
        {
            Content = new StringContent(
                                    Constants.Response.Json.Subscriptions,
                                    Encoding.UTF8,
                                    Constants.MediaType.Json
                                )
        };
        static HttpResponseMessage GetMockSubscription1ResourceGroupsResponse()
        => new()
        {
            Content = new StringContent(
                                    Constants.Response.Json.Subscription1.ResourceGroups,
                                    Encoding.UTF8,
                                    Constants.MediaType.Json
                                )
        };
        static HttpResponseMessage GetMockSubscription2ResourceGroupsResponse()
        => new()
        {
            Content = new StringContent(
                                    Constants.Response.Json.Subscription2.ResourceGroups,
                                    Encoding.UTF8,
                                    Constants.MediaType.Json
                                )
        };

        static MockHttpClient CreateClient()
            => new (
                    request => request switch
                    {
                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Tenants
                        } => GetMockTenantsResponse(),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.GraphOrg
                        } => GetMockGraphOrgResponse(),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscriptions
                        } => GetMockSubscriptionsResponse(),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscription1.ResourceGroups
                        } => GetMockSubscription1ResourceGroupsResponse(),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscription2.ResourceGroups
                        } => GetMockSubscription2ResourceGroupsResponse(),

                        _ => new HttpResponseMessage
                        {
                            StatusCode = System.Net.HttpStatusCode.NotFound
                        }
                    }
                );

        return services
            .AddSingleton<HttpClient>(
            _ => CreateClient()
            )
            .AddSingleton<IHttpClientFactory>(
             _ =>
             {
                 var httpClientFactory = Substitute.For<IHttpClientFactory>();
                 httpClientFactory
                    .CreateClient(null!)
                    .ReturnsForAnyArgs(_ => CreateClient());

                 httpClientFactory
                    .CreateClient()
                    .Returns(_ => CreateClient());

                 return httpClientFactory;
             }
            );;
    }
}
