using System.IO;

namespace Narchive.Formats
{
    internal abstract class NarcArchiveFileOrDirectoryEntry
    {
        internal string DirectoryName { get; set; } = string.Empty;

        internal string Name { get; set; } = string.Empty;

        internal string FullName => Path.Combine(DirectoryName, Name);
    }
}
