
using System.Collections.Generic;

namespace Config
{
    /// <summary>
    /// Settings related to the Ref Data API
    /// </summary>
    public class ApiSettings
    {
        public string BaseUri { get; set; } = string.Empty; // TODO: Validate that this is in fact a URI in service layer

        public Dictionary<string, string> RefDataEndpoints { get; set; } = new Dictionary<string, string>();
    }
}
