﻿using System;

namespace FileManagerDomain.Exceptions
{
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException() { }
        public AccessDeniedException(string message) : base(message) { }
        public AccessDeniedException(string message, Exception inner) 
            : base(message, inner) { }
    }
}