using Narchive.IO;
using System.IO;

namespace Narchive.Formats
{
    public class NarcArchiveEntry
    {
        private readonly Stream stream;
        private readonly string entryName;
        private readonly string name;
        private readonly int position;
        private readonly int length;

        internal NarcArchiveEntry(Stream stream, string entryName, int position, int length)
        {
            this.stream = stream;
            this.entryName = entryName;
            name = Path.GetFileName(entryName);
            this.position = position;
            this.length = length;
        }

        public string FullName => entryName;

        public int Length => length;

        public string Name => name;

        public Stream Open()
        {
            return new SubReadStream(stream, position, length);
        }
    }
}
