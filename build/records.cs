using System.Text.Json.Serialization;

/*****************************
 * Records
 *****************************/
public record BuildData(
    string Version,
    bool IsMainBranch,
    bool ShouldNotPublish,
    bool IsLocalBuild,
    DirectoryPath ProjectRoot,
    FilePath ProjectPath,
    DotNetMSBuildSettings MSBuildSettings,
    DirectoryPath ArtifactsPath,
    DirectoryPath OutputPath
    )
{
    private const string    IntegrationTest = "integrationtest",
                            Web = "web",
                            Output = "output";
    public DirectoryPath NuGetOutputPath { get; } = OutputPath.Combine("nuget");
    public DirectoryPath BinaryOutputPath { get; } = OutputPath.Combine("bin");
    public DirectoryPath IntegrationTestPath { get; } = OutputPath.Combine(IntegrationTest);
    public DirectoryPath StatiqWebPath { get; } = ArtifactsPath.Combine(Web);
    public DirectoryPath StatiqWebOutputPath { get; } = ArtifactsPath.Combine(Web).Combine(Output);

    public string? GitHubNuGetSource { get; } = System.Environment.GetEnvironmentVariable("GH_PACKAGES_NUGET_SOURCE");
    public string? GitHubNuGetApiKey { get; } = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");

    public bool ShouldPushGitHubPackages() =>   !ShouldNotPublish
                                                && !string.IsNullOrWhiteSpace(GitHubNuGetSource)
                                                && !string.IsNullOrWhiteSpace(GitHubNuGetApiKey);

    public string? NuGetSource { get; } = System.Environment.GetEnvironmentVariable("NUGET_SOURCE");
    public string? NuGetApiKey { get; } = System.Environment.GetEnvironmentVariable("NUGET_APIKEY");
    public bool ShouldPushNuGetPackages() =>    IsMainBranch &&
                                                !ShouldNotPublish &&
                                                !string.IsNullOrWhiteSpace(NuGetSource) &&
                                                !string.IsNullOrWhiteSpace(NuGetApiKey);

    public ICollection<DirectoryPath> DirectoryPathsToClean = new []{
        ArtifactsPath,
        ArtifactsPath.Combine(Web),
        ArtifactsPath.Combine(Web).Combine(Output),
        OutputPath,
        OutputPath.Combine(IntegrationTest)
    };

    public AzureCredentials AzureCredentials { get; } = new AzureCredentials(
                                                            System.Environment.GetEnvironmentVariable("AZURE_TENANT_ID"),
                                                            System.Environment.GetEnvironmentVariable("AZURE_CLIENT_ID"),
                                                            System.Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET"),
                                                            System.Environment.GetEnvironmentVariable("AZURE_AUTHORITY_HOST")
                                                        );

    public string? AzureTenantId { get; } = System.Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
    public string? AzureDomain { get; } = System.Environment.GetEnvironmentVariable("AZURE_DOMAIN");

    public bool ShouldRunIntegrationTests() =>  !string.IsNullOrWhiteSpace(AzureDomain) &&
                                                (
                                                    AzureCredentials.AzureCredentialsSpecified ||
                                                    IsLocalBuild
                                                );
}

public record AzureCredentials(
    string? TenantId,
    string? ClientId,
    string? ClientSecret,
    string? AuthorityHost = "login.microsoftonline.com"
)
{
    public bool AzureCredentialsSpecified { get; } = !string.IsNullOrWhiteSpace(TenantId) &&
                                                     !string.IsNullOrWhiteSpace(ClientId) &&
                                                     !string.IsNullOrWhiteSpace(ClientSecret) &&
                                                     !string.IsNullOrWhiteSpace(AuthorityHost);
}

internal record ExtensionHelper(Func<string, CakeTaskBuilder> TaskCreate, Func<CakeReport> Run);
