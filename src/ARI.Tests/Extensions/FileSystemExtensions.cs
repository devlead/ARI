using Cake.Core.IO;

namespace ARI.Tests.Extensions;

public static class FileSystemExtensions
{
    public static DirectoryResult FromDirectoryPath(this IFileSystem fileSystem, DirectoryPath directoryPath)
        => DirectoryResult.FromDirectoryPath(fileSystem, directoryPath);

    public record struct DirectoryResult(
        DirectoryPath Path,
        bool Exists,
        bool Hidden,
        DirectoryResult[] Directories,
        FileResult[] Files
        )
    {
        public static DirectoryResult FromDirectoryPath(IFileSystem fileSystem, DirectoryPath directoryPath)
                => FromDirectory(fileSystem.GetDirectory(directoryPath));

        public static DirectoryResult FromDirectory(IDirectory directory)
                 => new(
                     directory.Path,
                     directory.Exists,
                     directory.Hidden,
                     directory
                         .GetDirectories(string.Empty, SearchScope.Current)
                         .OrderBy(directories => directories.Path.FullPath, StringComparer.OrdinalIgnoreCase)
                         .Select(FromDirectory)
                         .ToArray(),
                     directory
                         .GetFiles(string.Empty, SearchScope.Current)
                         .OrderBy(file => file.Path.FullPath, StringComparer.OrdinalIgnoreCase)
                         .Select(FileResult.FromFile)
                         .ToArray()
                 );
    }

    public record struct FileResult(
        FilePath Path,
        bool Exists,
        bool Hidden,
        long Length
        )
    {
        public static FileResult FromFile(IFile file)
              => new(
                  file.Path,
                  file.Exists,
                  file.Hidden,
                  file
                    .ReadLines(Encoding.UTF8)
                    .SelectMany(c => c)
                    .LongCount()
                  );
    }
}