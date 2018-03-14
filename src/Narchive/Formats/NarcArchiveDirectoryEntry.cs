using System.Collections.Generic;

namespace Narchive.Formats
{
    public class NarcArchiveDirectoryEntry
    {
        /// <summary>
        /// Gets or sets the index of the directory entry.
        /// </summary>
        public virtual int Index { get; set; }

        /// <summary>
        /// Gets or sets the name of the directory.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets the relative path of the directory.
        /// </summary>
        public string FullName => Parent != null
            ? System.IO.Path.Combine(Parent.FullName, Name)
            : Name;

        /// <summary>
        /// Gets or sets the path, on the filesystem, of the directory.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the parent directory.
        /// </summary>
        public virtual NarcArchiveDirectoryEntry Parent { get; set; }

        /// <summary>
        /// Gets the sub-directories in the directory.
        /// </summary>
        public List<NarcArchiveDirectoryEntry> Directories { get; set; } = new List<NarcArchiveDirectoryEntry>();

        /// <summary>
        /// Gets the files in the directory.
        /// </summary>
        public List<NarcArchiveFileEntry> Files { get; set; } = new List<NarcArchiveFileEntry>();

        /// <summary>
        /// Gets or sets the offset of the directory name in the name entry table.
        /// </summary>
        internal virtual int NameEntryOffset { get; set; }

        /// <summary>
        /// Gets or sets the index of the first file in the directory.
        /// </summary>
        internal int FirstFileIndex { get; set; }
    }
}
