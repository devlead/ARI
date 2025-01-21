#addin "nuget:?package=xunit.assert&version=2.9.3"
#load "records.cake"

// Usings
using Xunit;

/*****************************
 * Helpers
 *****************************/

private static ExtensionHelper extensionHelper;
extensionHelper = new ExtensionHelper(Task, () => RunTarget(Argument("target", "Default")));
public static CakeTaskBuilder Then(this CakeTaskBuilder cakeTaskBuilder, string name)
    => extensionHelper
        .TaskCreate(name)
        .IsDependentOn(cakeTaskBuilder);


public static CakeReport Run(this CakeTaskBuilder cakeTaskBuilder)
    => extensionHelper.Run();

public static CakeTaskBuilder Default(this CakeTaskBuilder cakeTaskBuilder)
{
    extensionHelper
        .TaskCreate("Default")
        .IsDependentOn(cakeTaskBuilder);
    return cakeTaskBuilder;
}

if (BuildSystem.GitHubActions.IsRunningOnGitHubActions)
{
    TaskSetup(context=> System.Console.WriteLine($"::group::{context.Task.Name.Quote()}"));
    TaskTeardown(context=>System.Console.WriteLine("::endgroup::"));
}

public class FilePathJsonConverter : PathJsonConverter<FilePath>
{
    protected override FilePath ConvertFromString(string value) => FilePath.FromString(value);
}

public abstract class PathJsonConverter<TPath> : System.Text.Json.Serialization.JsonConverter<TPath> where TPath : Cake.Core.IO.Path
{
    public override TPath Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
        var value = reader.GetString();

        return value is null ? null : ConvertFromString(value);
    }

    public override void Write(System.Text.Json.Utf8JsonWriter writer, TPath value, System.Text.Json.JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.FullPath);
    }

    protected abstract TPath ConvertFromString(string value);
}