using Cake.Common.IO;

namespace ARI.Extensions;
public static class CakeContextExtension
{
    public static TextWriter OpenIndexWrite(
        this ICakeContext cakeContext,
        DirectoryPath parentPath,
        AzureResourceBase azureResource,
        string markDownFileName,
        out DirectoryPath targetPath
        )
        => cakeContext.OpenIndexWrite(parentPath, azureResource.PublicId, markDownFileName, out targetPath);
    

    public static TextWriter OpenIndexWrite(
        this ICakeContext cakeContext,
        DirectoryPath parentPath,
        string publicId,
        string markDownFileName,
        out DirectoryPath targetPath
        )
    {
        targetPath = parentPath.Combine(publicId.PathEscapeUriString());

        return cakeContext.OpenIndexWrite(targetPath, markDownFileName);
    }

    public static TextWriter OpenIndexWrite(
        this ICakeContext cakeContext,
        DirectoryPath targetPath,
        string markDownFileName
        )
    {
        lock (cakeContext.FileSystem)
        {
            cakeContext.EnsureDirectoryExists(targetPath);
        }

        var stream = cakeContext
                        .FileSystem
                        .GetFile(targetPath.CombineWithFilePath(markDownFileName))
                        .OpenWrite();

        var writer = new StreamWriter(
            stream,
            Encoding.UTF8
        );

        return writer;
    }
}
