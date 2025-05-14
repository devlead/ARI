#tool dotnet:?package=GitVersion.Tool&version=6.3.0
#load "build/records.cake"
#load "build/helpers.cake"

/*****************************
 * Setup
 *****************************/
Setup(
    static context => {
         var assertedVersions = context.GitVersion(new GitVersionSettings
            {
                OutputType = GitVersionOutput.Json
            });

        var branchName = assertedVersions.BranchName;
        var isMainBranch = StringComparer.OrdinalIgnoreCase.Equals("main", branchName);

        var gh = context.GitHubActions();
        var buildDate = DateTime.UtcNow;
        var runNumber = gh.IsRunningOnGitHubActions
                            ? gh.Environment.Workflow.RunNumber
                            : (short)((buildDate - buildDate.Date).TotalSeconds/3);

        var version = FormattableString
                    .Invariant($"{buildDate:yyyy.M.d}.{runNumber}");

        context.Information("Building version {0} (Branch: {1}, IsMain: {2})",
            version,
            branchName,
            isMainBranch);

        var artifactsPath = context
                            .MakeAbsolute(context.Directory("./artifacts"));

        var projectRoot =  context
                            .MakeAbsolute(context.Directory("./src"));

        var projectPath = projectRoot.CombineWithFilePath("ARI/ARI.csproj");

        return new BuildData(
            version,
            isMainBranch,
            !context.IsRunningOnWindows(),
            context.BuildSystem().IsLocalBuild,
            projectRoot,
            projectPath,
            new DotNetMSBuildSettings()
                .SetConfiguration("Release")
                .SetVersion(version)
                .WithProperty("Copyright", $"Mattias Karlsson © {DateTime.UtcNow.Year}")
                .WithProperty("Authors", "devlead")
                .WithProperty("Company", "devlead")
                .WithProperty("PackageLicenseExpression", "MIT")
                .WithProperty("PackageTags", "tool;bicep;acr;azure")
                .WithProperty("PackageDescription", "Azure Resource Inventory .NET Tool - Inventories and documents Azure Tenant resources")
                .WithProperty("RepositoryUrl", "https://github.com/devlead/ARI.git")
                .WithProperty("ContinuousIntegrationBuild", gh.IsRunningOnGitHubActions ? "true" : "false")
                .WithProperty("EmbedUntrackedSources", "true"),
            artifactsPath,
            artifactsPath.Combine(version)
            );
    }
);

/*****************************
 * Tasks
 *****************************/
Task("Clean")
    .Does<BuildData>(
        static (context, data) => context.CleanDirectories(data.DirectoryPathsToClean)
    )
.Then("Restore")
    .Does<BuildData>(
        static (context, data) => context.DotNetRestore(
            data.ProjectRoot.FullPath,
            new DotNetRestoreSettings {
                MSBuildSettings = data.MSBuildSettings
            }
        )
    )
.Then("DPI")
    .Does<BuildData>(
        static (context, data) => context.DotNetTool(
                "tool",
                new DotNetToolSettings {
                    ArgumentCustomization = args => args
                                                        .Append("run")
                                                        .Append("dpi")
                                                        .Append("nuget")
                                                        .Append("--silent")
                                                        .AppendSwitchQuoted("--output", "table")
                                                        .Append(
                                                            (
                                                                !string.IsNullOrWhiteSpace(context.EnvironmentVariable("NuGetReportSettings_SharedKey"))
                                                                &&
                                                                !string.IsNullOrWhiteSpace(context.EnvironmentVariable("NuGetReportSettings_WorkspaceId"))
                                                            )
                                                                ? "report"
                                                                : "analyze"
                                                            )
                                                        .AppendSwitchQuoted("--buildversion", data.Version)
                }
            )
    )
.Then("Build")
    .Does<BuildData>(
        static (context, data) => context.DotNetBuild(
            data.ProjectRoot.FullPath,
            new DotNetBuildSettings {
                NoRestore = true,
                MSBuildSettings = data.MSBuildSettings
            }
        )
    )
.Then("Test")
    .Does<BuildData>(
        static (context, data) => context.DotNetTest(
            data.ProjectRoot.FullPath,
            new DotNetTestSettings {
                NoBuild = true,
                NoRestore = true,
                MSBuildSettings = data.MSBuildSettings
            }
        )
    )
.Then("Pack")
    .Does<BuildData>(
        static (context, data) => context.DotNetPack(
            data.ProjectPath.FullPath,
            new DotNetPackSettings {
                NoBuild = true,
                NoRestore = true,
                OutputDirectory = data.NuGetOutputPath,
                MSBuildSettings = data.MSBuildSettings
            }
        )
    )
.Then("Upload-Artifacts")
    .WithCriteria(BuildSystem.IsRunningOnGitHubActions, nameof(BuildSystem.IsRunningOnGitHubActions))
    .Does<BuildData>(
        static (context, data) => context
            .GitHubActions() is var gh && gh != null
                ?   gh.Commands
                    .UploadArtifact(data.ArtifactsPath,  $"Artifact_{gh.Environment.Runner.ImageOS ?? gh.Environment.Runner.OS}_{context.Environment.Runtime.BuiltFramework.Identifier}_{context.Environment.Runtime.BuiltFramework.Version}")
                : throw new Exception("GitHubActions not available")
    )
.Then("Integration-Tests-Tool-Manifest")
    .Does<BuildData>(
        static (context, data) => context.DotNetTool(
                "new",
                new DotNetToolSettings {
                    ArgumentCustomization = args => args
                                                        .Append("tool-manifest"),
                    WorkingDirectory = data.IntegrationTestPath
                }
            )
    )
.Then("Integration-Tests-Tool-Install")
    .Does<BuildData>(
        static (context, data) =>  context.DotNetTool(
                "tool",
                new DotNetToolSettings {
                    ArgumentCustomization = args => args
                                                        .Append("install")
                                                        .AppendSwitchQuoted("--add-source", data.NuGetOutputPath.FullPath)
                                                        .AppendSwitchQuoted("--version", data.Version)
                                                        .Append("ARI"),
                    WorkingDirectory = data.IntegrationTestPath
                }
            )
    )
.Then("Integration-Tests-Tool")
    .WithCriteria<BuildData>((context, data) => data.ShouldRunIntegrationTests(), "ShouldRunIntegrationTests")
    .Does<BuildData>(
        static (context, data) => context.DotNetTool(
                "tool",
                new DotNetToolSettings {
                    ArgumentCustomization = args => args
                                                        .Append("run")
                                                        .Append("--")
                                                        .Append("ARI")
                                                        .Append("inventory")
                                                        .AppendQuotedSecret(data.AzureTenantId)
                                                        .AppendQuoted(data.IntegrationTestPath.FullPath)
                                                        .Append("--skip-tenant-overview")
                                                        .Append("--include-site-application-settings"),
                    WorkingDirectory = data.IntegrationTestPath
                }
            )
    )
.Then("Integration-Tests-Upload-Results")
    .WithCriteria(BuildSystem.IsRunningOnGitHubActions, nameof(BuildSystem.IsRunningOnGitHubActions))
    .WithCriteria<BuildData>((context, data) => data.ShouldRunIntegrationTests(), "ShouldRunIntegrationTests")
    .Does<BuildData>(
         async (context, data) => {
            var resultPath = data.IntegrationTestPath;
            await GitHubActions.Commands.UploadArtifact(
                resultPath,
                $"{data.AzureDomain}_{GitHubActions.Environment.Runner.ImageOS ?? GitHubActions.Environment.Runner.OS}_{context.Environment.Runtime.BuiltFramework.Identifier}_{context.Environment.Runtime.BuiltFramework.Version}"
            );
            GitHubActions.Commands.SetStepSummary(
                string.Join(
                    System.Environment.NewLine,
                    context.GetFiles($"{resultPath.FullPath}/**/*.md")
                        .Select(filePath => context.FileSystem.GetFile(filePath).ReadLines(Encoding.UTF8))
                        .SelectMany(line => line)
                )
            );
         }
    )
.Then("Integration-Tests")
.Then("Generate-Statiq-Web")
    .WithCriteria<BuildData>((context, data) => data.ShouldRunIntegrationTests(), "ShouldRunIntegrationTests")
    .WithCriteria<BuildData>((context, data) => !context.IsRunningOnMacOs(), "Not IsRunningOnMacOS")
    .Does<BuildData>(static (context, data) => {
        context.DotNetRun(
            data.ProjectRoot.CombineWithFilePath("ARI.TestWeb/ARI.TestWeb.csproj").FullPath,
            new DotNetRunSettings {
                Configuration = "Release",
                NoBuild = true,
                NoRestore = true,
                WorkingDirectory = data.StatiqWebPath,
                ArgumentCustomization = args => args
                                                    .Append(context.Argument("preview", false) ? "-- preview --virtual-dir ARI/" : "--")
                                                    .AppendSwitchQuoted("--root", data.StatiqWebPath.FullPath)
                                                    .AppendSwitchQuoted("--input", data.IntegrationTestPath.FullPath)
                                                    .AppendSwitchQuoted("--output", data.StatiqWebOutputPath.FullPath)
            }
        );
    })
    .Default()
.Then("Push-GitHub-Packages")
    .WithCriteria<BuildData>( (context, data) => data.ShouldPushGitHubPackages())
    .DoesForEach<BuildData, FilePath>(
        static (data, context)
            => context.GetFiles(data.NuGetOutputPath.FullPath + "/*.nupkg"),
        static (data, item, context)
            => context.DotNetNuGetPush(
                item.FullPath,
            new DotNetNuGetPushSettings
            {
                Source = data.GitHubNuGetSource,
                ApiKey = data.GitHubNuGetApiKey
            }
        )
    )
.Then("Push-NuGet-Packages")
    .WithCriteria<BuildData>( (context, data) => data.ShouldPushNuGetPackages())
    .DoesForEach<BuildData, FilePath>(
        static (data, context)
            => context.GetFiles(data.NuGetOutputPath.FullPath + "/*.nupkg"),
        static (data, item, context)
            => context.DotNetNuGetPush(
                item.FullPath,
                new DotNetNuGetPushSettings
                {
                    Source = data.NuGetSource,
                    ApiKey = data.NuGetApiKey
                }
        )
    )
.Then("Create-GitHub-Release")
    .WithCriteria<BuildData>( (context, data) => data.ShouldPushNuGetPackages())
    .Does<BuildData>(
        static (context, data) => context
            .Command(
                new CommandSettings {
                    ToolName = "GitHub CLI",
                    ToolExecutableNames = new []{ "gh.exe", "gh" },
                    EnvironmentVariables = { { "GH_TOKEN", data.GitHubNuGetApiKey } }
                },
                new ProcessArgumentBuilder()
                    .Append("release")
                    .Append("create")
                    .Append(data.Version)
                    .AppendSwitchQuoted("--title", data.Version)
                    .Append("--generate-notes")
                    .Append(string.Join(
                        ' ',
                        context
                            .GetFiles(data.NuGetOutputPath.FullPath + "/*.nupkg")
                            .Select(path => path.FullPath.Quote())
                        ))

            )
    )
.Then("GitHub-Actions")
.Run();
