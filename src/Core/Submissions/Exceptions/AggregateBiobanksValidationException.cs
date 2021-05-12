using System;
using System.Collections.Generic;

namespace Core.Submissions.Exceptions
{
    public class AggregateBiobanksValidationException : Exception
    {
        public ICollection<BiobanksValidationResult> ValidationResults { get; }

        public AggregateBiobanksValidationException(ICollection<BiobanksValidationResult> validationResults)
        {
            ValidationResults = validationResults;
        }
    }

    public class BiobanksValidationResult
    {
        public string ErrorMessage { get; set; }
        public string RecordIdentifiers { get; set; }
    }
}
