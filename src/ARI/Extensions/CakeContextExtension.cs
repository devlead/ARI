using Cake.Common.IO;

namespace ARI.Extensions;
public static class CakeContextExtension
{
    public static TextWriter OpenIndexWrite(
        this ICakeContext cakeContext,
        DirectoryPath targetPath
        )
    {
        cakeContext.EnsureDirectoryExists(targetPath);

        var stream = cakeContext
                        .FileSystem
                        .GetFile(targetPath.CombineWithFilePath("index.md"))
                        .OpenWrite();

        var writer = new StreamWriter(
            stream,
            Encoding.UTF8
        );

        return writer;
    }
}
