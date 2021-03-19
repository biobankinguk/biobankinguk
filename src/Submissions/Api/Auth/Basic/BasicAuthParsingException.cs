using System;

namespace Biobanks.Submissions.Api.Auth.Basic
{
    internal class BasicAuthParsingException : Exception
    {
        public BasicAuthParsingException(string message) : base(message) { }

        public BasicAuthParsingException(string message, Exception innerException) : base(message, innerException) { }
    }
}