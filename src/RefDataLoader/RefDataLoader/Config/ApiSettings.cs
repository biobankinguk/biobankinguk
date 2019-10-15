
using System.Collections.Generic;

namespace Config
{
    /// <summary>
    /// Settings related to the Ref Data API
    /// </summary>
    public class ApiSettings
    {
        public string BaseUri { get; set; }

        public Dictionary<string, string> RefDataEndpoints { get; set; }
    }
}
