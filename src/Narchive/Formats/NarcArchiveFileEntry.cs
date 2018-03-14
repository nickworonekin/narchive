namespace Narchive.Formats
{
    public class NarcArchiveFileEntry
    {
        /// <summary>
        /// Gets or sets the index of the file entry.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the relative path of the file.
        /// </summary>
        public string FullName => Directory != null
            ? System.IO.Path.Combine(Directory.FullName, Name)
            : Name;

        /// <summary>
        /// Gets or sets the path, on the filesystem, of the file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the <seealso cref="DirectoryEntry"/> this file belongs to.
        /// </summary>
        public NarcArchiveDirectoryEntry Directory { get; set; }

        /// <summary>
        /// Gets or sets the offset of the file data.
        /// </summary>
        internal int Offset { get; set; }

        /// <summary>
        /// Gets or sets the length of the file data.
        /// </summary>
        internal int Length { get; set; }
    }
}
