using System;

namespace Front_End.Exceptions
{
    class BackendTimeoutException : Exception
    {
        public BackendTimeoutException()
        {
        }

        public BackendTimeoutException(string message)
        : base(message)
        {
        }

        public BackendTimeoutException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}