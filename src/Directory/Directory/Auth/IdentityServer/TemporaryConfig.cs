using System.Collections.Generic;
using IdentityServer4.Test;

namespace Directory.Auth.IdentityServer
{
    public static class TemporaryConfig
    {
        /// <summary>
        /// Temporary Test Users until Identity is implemented
        /// </summary>
        // TODO: Remove when ready
        public static List<TestUser> GetUsers()
            => new List<TestUser>
            {
                new TestUser { SubjectId = "1", Username = "jon@jon.jon", Password="test"},
                new TestUser { SubjectId = "2", Username = "bob@bob.bob", Password="test"}
            };
    }
}
