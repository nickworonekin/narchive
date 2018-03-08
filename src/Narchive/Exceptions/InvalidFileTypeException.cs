using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
