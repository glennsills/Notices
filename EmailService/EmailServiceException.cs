using System;

namespace Notices.EmailService
{
    public class EmailServiceException: Exception
    {
        public EmailServiceException(string message): base(message){}
        public EmailServiceException(Exception inner): base("Exception caught by EmailService", inner){}
    }
}