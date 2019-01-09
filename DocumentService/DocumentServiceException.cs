using System;
using System.Runtime.Serialization;

namespace Notices.DocumentService
{
    [Serializable]
    internal class DocumentServiceException : Exception
    {
        public DocumentServiceException()
        {
        }

        public DocumentServiceException(string message) : base(message)
        {
        }

        public DocumentServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DocumentServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}