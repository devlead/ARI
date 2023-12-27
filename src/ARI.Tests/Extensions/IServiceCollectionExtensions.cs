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
        static HttpResponseMessage GetMockJsonResponse(string response)
        => new()
        {
            Content = new StringContent(
                                    response,
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
                        } => GetMockJsonResponse(Constants.Response.Json.TenantsServicePrinciple),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.GraphOrg
                        } => GetMockJsonResponse(Constants.Response.Json.GraphOrg),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscriptions
                        } => GetMockJsonResponse(Constants.Response.Json.Subscriptions),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscription1.ResourceGroups
                        } => GetMockJsonResponse(Constants.Response.Json.Subscription1.ResourceGroups),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscription2.ResourceGroups
                        } => GetMockJsonResponse(Constants.Response.Json.Subscription2.ResourceGroups),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscription1.ResourceGroup1.Resources
                        } => GetMockJsonResponse(Constants.Response.Json.Subscription1.ResourceGroup1.Resources),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscription1.ResourceGroup2.Resources
                        } => GetMockJsonResponse(Constants.Response.Json.Subscription1.ResourceGroup2.Resources),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscription1.ResourceGroup3.Resources
                        } => GetMockJsonResponse(Constants.Response.Json.Subscription1.ResourceGroup3.Resources),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscription2.ResourceGroup1.Resources
                        } => GetMockJsonResponse(Constants.Response.Json.Subscription2.ResourceGroup1.Resources),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscription1.ResourceGroup1.Site.Config
                        } => GetMockJsonResponse(Constants.Response.Json.Subscription1.ResourceGroup1.Site.Config),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscription1.ResourceGroup2.Site.Config
                        } => GetMockJsonResponse(Constants.Response.Json.Subscription1.ResourceGroup2.Site.Config),

                        {
                            Method.Method: Constants.Request.Method.Get,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscription1.ResourceGroup3.Site.Config
                        } => GetMockJsonResponse(Constants.Response.Json.Subscription1.ResourceGroup3.Site.Config),

                        {
                            Method.Method: Constants.Request.Method.Post,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscription1.ResourceGroup1.Site.Settings
                        } => GetMockJsonResponse(Constants.Response.Json.Subscription1.ResourceGroup1.Site.Settings),

                        {
                            Method.Method: Constants.Request.Method.Post,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscription1.ResourceGroup2.Site.Settings
                        } => GetMockJsonResponse(Constants.Response.Json.Subscription1.ResourceGroup2.Site.Settings),

                        {
                            Method.Method: Constants.Request.Method.Post,
                            RequestUri.AbsoluteUri: Constants.Request.Uri.Subscription1.ResourceGroup3.Site.Settings
                        } => GetMockJsonResponse(Constants.Response.Json.Subscription1.ResourceGroup3.Site.Settings),

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
