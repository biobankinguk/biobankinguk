using System.Collections.Generic;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.ADAC
{
    public class ReadOntologyTermModel : OntologyTermModel
    {
        //Sum of all Collections and Capabilities
        public int CollectionCapabilityCount { get; set; }

    }
}