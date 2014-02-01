using System;
using System.Net;

namespace Public311.API.Common
{
    public class ServiceException : WebException
    {
        public ServiceException()
            : base()
        {
        }

        public ServiceException(string message)
            : base(message)
        {
        }

        public ServiceException(string message, WebException innerException)
            : base(message, innerException)
        {
        }
    }
}
