using System.Collections.Generic;
using System.IO;

namespace Narchive.Formats
{
    public class NarcArchiveRootDirectoryEntry : NarcArchiveDirectoryEntry
    {
        /// <summary>
        /// Gets the index of the root directory, which is always 0.
        /// </summary>
        public override int Index => 0;

        /// <summary>
        /// Gets the name of the root directory, which is always an empty string.
        /// </summary>
        public override string Name => string.Empty;

        /// <summary>
        /// Gets the parent directory, which is always null for the root directory.
        /// </summary>
        public override NarcArchiveDirectoryEntry Parent => null;

        /// <summary>
        /// Creates a <seealso cref="NarcArchiveRootDirectoryEntry"/> from a specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A <seealso cref="NarcArchiveRootDirectoryEntry"/>.</returns>
        public static NarcArchiveRootDirectoryEntry CreateFromPath(string path)
        {
            var rootDirectory = new NarcArchiveRootDirectoryEntry
            {
                Path = path,
            };

            var directoryEntries = new Queue<NarcArchiveDirectoryEntry>();
            directoryEntries.Enqueue(rootDirectory);

            while (directoryEntries.Count > 0)
            {
                var currentDirectoryEntry = directoryEntries.Dequeue();

                var fileSystemEntries = Directory.EnumerateFileSystemEntries(currentDirectoryEntry.Path);
                foreach (var fileSystemEntry in fileSystemEntries)
                {
                    var isDirectory = File.GetAttributes(fileSystemEntry).HasFlag(FileAttributes.Directory);
                    if (isDirectory)
                    {
                        var directoryEntry = new NarcArchiveDirectoryEntry
                        {
                            Name = new DirectoryInfo(fileSystemEntry).Name,
                            Path = fileSystemEntry,
                            Parent = currentDirectoryEntry,
                        };

                        currentDirectoryEntry.Entries.Add(directoryEntry);
                        directoryEntries.Enqueue(directoryEntry);
                    }
                    else
                    {
                        var fileEntry = new NarcArchiveFileEntry
                        {
                            Name = new FileInfo(fileSystemEntry).Name,
                            Path = fileSystemEntry,
                            Directory = currentDirectoryEntry,
                        };

                        currentDirectoryEntry.Entries.Add(fileEntry);
                    }
                }
            }

            return rootDirectory;
        }
    }
}
