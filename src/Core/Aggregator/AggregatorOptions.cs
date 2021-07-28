using System;
using System.Collections.Generic;

namespace Biobanks.Aggregator
{
    public class AggregatorOptions
    {
        //private readonly IDictionary<Tuple<string, string>, string> _mappings = new Dictionary<Tuple<string, string>, string>()
        //{
        //    { new Tuple<string, string>("Macroscopic Assessment", null), "Affected" },
        //    { new Tuple<string, string>("Microscopic Assessment", null), "Affected" },

        //    { new Tuple<string, string>("Macroscopic Assessment", "102499006"), "Non-Affected" }, // Fit and Well
        //    { new Tuple<string, string>("Macroscopic Assessment", "102499006"), "Non-Affected" },
        //    { new Tuple<string, string>("Microscopic Assessment", "23875004"), "Non-Affected-" }, // No Pathological Disease
        //    { new Tuple<string, string>("Microscopic Assessment", "23875004"), "Non-Affected-" },

        //    { new Tuple<string, string>(null, null), "Not Applicable" },
        //};

        /// <summary>
        /// The OntologyTerm used in collections for Non-Extracted Samples
        /// </summary>
        public readonly string NonExtractedOntologyTerm = "102499006"; // Fit and Well

        /// <summary>
        /// Mappings of (contentMethod, contentId) => macroscopicAssessment
        /// </summary>
        private readonly IDictionary<(string method, string id), string> _mappings;

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
            (string method, string id) fullKey = (contentMethod, contentId);    // Specific Mapping
            (string method, string id) partialKey = (contentMethod, null);      // General Mapping (No Id)
            (string method, string id) emptyKey = (null, null);                 // Default Mapping (No Method Or Id)

            // Try Find Specific Mapping
            if (_mappings.TryGetValue(fullKey, out var macro))
                return macro;

            // Fallback To General ContentMethod Mapping
            if (_mappings.TryGetValue(partialKey, out macro))
                return macro;

            // Fallback To Default Value
            return _mappings[emptyKey];
        }
    }
}
