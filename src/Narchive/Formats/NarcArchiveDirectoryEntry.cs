using System.Collections.Generic;

namespace Narchive.Formats
{
    internal class NarcArchiveDirectoryEntry : NarcArchiveFileOrDirectoryEntry
    {
        internal int Index { get; set; }

        internal int ParentDirectoryIndex { get; set; }

        internal int NameEntryOffset { get; set; }

        internal int FirstFileIndex { get; set; }

        internal List<NarcArchiveFileOrDirectoryEntry> Entries { get; set; } = new List<NarcArchiveFileOrDirectoryEntry>();
    }
}
