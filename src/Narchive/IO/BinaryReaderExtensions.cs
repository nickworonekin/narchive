using System.IO;

namespace Narchive.IO
{
    public static class BinaryReaderExtensions
    {
        public static string ReadString(this BinaryReader reader, int count)
        {
            var chars = reader.ReadChars(count);
            return new string(chars);
        }
    }
}
