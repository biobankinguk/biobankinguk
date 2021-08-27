using System;
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
        /// All configured mappings between (ContentMethod, ContentId) of a Sample => MacroscopicAssessment
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
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when the mapping can not produce a valid MacroscopicAssessment, due to a missing mapping configuration
        /// </exception>
        public string MapToMacroscopicAssessment(string contentMethod, string contentId)
        {
            // Find Mapping - Using Fallback Values If No Direct Mapping Exists
            var map = 
                MacroscopicAssessmentMappings?.FirstOrDefault(x => x.ContentMethod == contentMethod && x.ContentId == contentId) ??       
                MacroscopicAssessmentMappings?.FirstOrDefault(x => x.ContentMethod == contentMethod && string.IsNullOrEmpty(x.ContentId)) ??   
                MacroscopicAssessmentMappings?.FirstOrDefault(x => string.IsNullOrEmpty(x.ContentMethod) && string.IsNullOrEmpty(x.ContentId));

            // Current Mapping Configuration Doesn't Cover All Cases
            if (map is null)
                throw new InvalidOperationException("MacroscopicAssessment mapping not properly configured.");

            return map.MacroscopicAssessment;
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
