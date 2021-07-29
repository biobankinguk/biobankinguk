using System.Collections.Generic;
using System.Linq;

namespace Biobanks.Aggregator
{
    public class AggregatorOptions
    {
        /// <summary>
        /// The OntologyTerm used in collections for Non-Extracted Samples
        /// </summary>
        public string NonExtractedOntologyTerm { get; set; }

        /// <summary>
        /// </summary>
        public IList<MacroscopicAssessmentMapping> MacroscopicAssessmentMappings { get; set; }

        /// <summary>
        /// Maps the ContentMethod and ContentId of a Sample to a MacroscopicAssessment using
        /// configured mappings.
        /// 
        /// It will attempt to find an exact mapping for the ContentMethod and ContentId, with fallback
        /// mappings for using just the ContentMethod or a configured default value.
        /// </summary>
        /// <returns>The mapped MacroscopicAssessment value</returns>
        public string MapToMacroscopicAssessment(string contentMethod, string contentId)
        {
            var map = MacroscopicAssessmentMappings.FirstOrDefault(x => x.ContentMethod == contentMethod && x.ContentId == contentId)       // Specific Mapping
                ?? MacroscopicAssessmentMappings.FirstOrDefault(x => x.ContentMethod == contentMethod && string.IsNullOrEmpty(contentId))   // General Mapping (No Id)
                ?? MacroscopicAssessmentMappings.First(x => string.IsNullOrEmpty(contentMethod) && string.IsNullOrEmpty(contentId));        // Default Mapping (No Method Or Id)

            return map?.MacroscopicAssessment;
        }
    }

    /// <summary>
    /// Represents a mapping between (ContentMethod, ContentId) => MacroscopicAssessment
    /// </summary>
    public class MacroscopicAssessmentMapping
    {
        public string ContentMethod { get; set; }
        public string ContentId { get; set; }
        public string MacroscopicAssessment { get; set; }
    }
}
