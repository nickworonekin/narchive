namespace Narchive.Formats
{
    internal class NarcArchiveFileEntry : NarcArchiveFileOrDirectoryEntry
    {
        internal int DirectoryIndex { get; set; }

        internal int Offset { get; set; }

        internal int Length { get; set; }
    }
}
