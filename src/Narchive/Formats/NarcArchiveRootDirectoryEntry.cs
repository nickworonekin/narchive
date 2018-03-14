using Narchive.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Gets or sets the offset of the root directory name in the name entry table, which is always 0.
        /// </summary>
        internal override int NameEntryOffset => 0;

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

                var directories = Directory.EnumerateDirectories(currentDirectoryEntry.Path);
                foreach (var directory in directories)
                {
                    var directoryEntry = new NarcArchiveDirectoryEntry
                    {
                        Name = new DirectoryInfo(directory).Name,
                        Path = directory,
                        Parent = currentDirectoryEntry,
                    };

                    currentDirectoryEntry.Directories.Add(directoryEntry);
                    directoryEntries.Enqueue(directoryEntry);
                }

                var files = Directory.EnumerateFiles(currentDirectoryEntry.Path);
                foreach (var file in files)
                {
                    var fileEntry = new NarcArchiveFileEntry
                    {
                        Name = new FileInfo(file).Name,
                        Path = file,
                        Directory = currentDirectoryEntry,
                    };

                    currentDirectoryEntry.Files.Add(fileEntry);
                }
            }

            return rootDirectory;
        }
    }
}
