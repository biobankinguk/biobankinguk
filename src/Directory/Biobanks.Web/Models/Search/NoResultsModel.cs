using System.Collections.Generic;
using Directory.Search.Constants;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.Search
{
    public class NoResultsModel
    {
        public SearchDocumentType SearchType { get; set; }
        public string Diagnosis { get; set; }

        public ICollection<DiagnosisModel> Suggestions { get; set; }
    }
}
