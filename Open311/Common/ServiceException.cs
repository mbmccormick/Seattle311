using System;
using System.Net;

namespace Open311.Common
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
