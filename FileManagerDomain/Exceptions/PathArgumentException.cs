using System;
using System.Collections.Generic;
using System.Text;

namespace FileManagerDomain.Exceptions
{
    public class PathArgumentException : ArgumentException
    {
        public PathArgumentException() { }
        public PathArgumentException(string message) : base(message) { }
        public PathArgumentException(string message, Exception inner) 
            : base(message, inner) { }
    }
}
