using Cake.Core;
using Cake.Core.Configuration;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

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
}
