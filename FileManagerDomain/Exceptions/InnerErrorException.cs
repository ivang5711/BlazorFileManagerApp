using System;

namespace FileManagerDomain.Exceptions
{
    public class InnerErrorException : Exception
    {
        public InnerErrorException() { }
        public InnerErrorException(string message) : base(message) { }
        public InnerErrorException(string message, Exception inner) : base(message, inner) { }
    }
}