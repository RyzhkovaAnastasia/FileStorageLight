using System;
using System.Runtime.Serialization;

namespace BLL.Exceptions
{
    [Serializable]
    public class DirectoryException : Exception
    {
        public DirectoryException()
        {
        }

        public DirectoryException(string message) : base(message)
        {
        }

        public DirectoryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DirectoryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
