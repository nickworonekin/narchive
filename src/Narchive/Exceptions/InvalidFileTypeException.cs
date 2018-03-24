using System;
using System.Runtime.Serialization;

namespace Narchive.Exceptions
{
    public class InvalidFileTypeException : Exception
    {
        public InvalidFileTypeException()
        {
        }

        public InvalidFileTypeException(string message) : base(message)
        {
        }

        public InvalidFileTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidFileTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
