using System;

namespace Biobanks.Directory.Data.Transforms.Url
{
    public class InvalidUrlSchemeException : Exception
    {
        public InvalidUrlSchemeException() { }
        public InvalidUrlSchemeException(string message) : base(message) { }
    }
}
