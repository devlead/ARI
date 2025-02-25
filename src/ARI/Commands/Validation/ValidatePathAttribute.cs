namespace ARI.Commands.Validation;

public class ValidatePathAttribute() : ParameterValidationAttribute(null!)
{
    public override ValidationResult Validate(CommandParameterContext context) 
    {
        // Resolve the file system from the context, throw if not found
        var fileSystem = context.GetRequiredService<IFileSystem>();

        // Check if the provided value is a valid file or directory path
        return context.Value switch
            {
                // If it's a file path and exists, validation succeeds
                FilePath filePath when fileSystem.Exist(filePath)
                    => ValidationResult.Success(),

                // If it's a directory path and exists, validation succeeds  
                DirectoryPath directoryPath when fileSystem.Exist(directoryPath)
                    => ValidationResult.Success(),

                // Otherwise return validation error with details
                _ => ValidationResult.Error($"Invalid {context.Parameter?.PropertyName} ({context.Value}) specified.")
            };
    }
}