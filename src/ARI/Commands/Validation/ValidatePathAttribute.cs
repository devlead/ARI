namespace ARI.Commands.Validation;

public class ValidatePathAttribute : ParameterValidationAttribute
{
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    public ValidatePathAttribute() : base(null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    {
    }

    public override ValidationResult Validate(CommandParameterContext context) => context.Value switch
    {
        FilePath filePath when File.Exists(filePath.FullPath)
            => ValidationResult.Success(),

        DirectoryPath directoryPath when Directory.Exists(directoryPath.FullPath)
            => ValidationResult.Success(),

        _ => ValidationResult.Error($"Invalid {context.Parameter?.PropertyName} ({context.Value}) specified.")
    };
}
