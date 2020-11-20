using System;

namespace Directory.Search.Exceptions
{
    public class InvalidIndexResponseException : Exception
    {
        public InvalidIndexResponseException(string message) : base(message) { }
    }
}
