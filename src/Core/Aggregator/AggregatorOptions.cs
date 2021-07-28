using System;
using System.Collections.Generic;

namespace Biobanks.Aggregator
{
    public class AggregatorOptions
    {

        private readonly IDictionary<Tuple<string, string>, string> _mappings = new Dictionary<Tuple<string, string>, string>()
        {
            { new Tuple<string, string>("Macroscopic Assessment", null), "Affected" },
            { new Tuple<string, string>("Microscopic Assessment", null), "Affected" },

            { new Tuple<string, string>("Macroscopic Assessment", "102499006"), "Non-Affected" }, // Fit and Well
            { new Tuple<string, string>("Macroscopic Assessment", "102499006"), "Non-Affected" },
            { new Tuple<string, string>("Microscopic Assessment", "23875004"), "Non-Affected-" }, // No Pathological Disease
            { new Tuple<string, string>("Microscopic Assessment", "23875004"), "Non-Affected-" },

            { new Tuple<string, string>(null, null), "Not Applicable" },
        };

        public string MapToMicroscopicAssessment(string contentMethod, string contentId)
        {
            var fullKey = new Tuple<string, string>(contentMethod, contentId);
            var partialKey = new Tuple<string, string>(contentMethod, null);
            var emptyKey = new Tuple<string, string>(null, null);

            if (_mappings.TryGetValue(fullKey, out var macro))
                return macro;

            if (_mappings.TryGetValue(partialKey, out macro))
                return macro;

            return _mappings[emptyKey];
        }
    }
}
