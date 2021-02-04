using System;

namespace Biobanks.Search.Exceptions
{
    public class InvalidIndexResponseException : Exception
    {
        public InvalidIndexResponseException(string message) : base(message) { }
    }
}
