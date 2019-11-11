namespace Biobanks.Common.Models
{
    public class TreatmentModel : TreatmentIdModel
    {
        public string TreatmentLocation { get; set; } = string.Empty;
        public string TreatmentCodeOntology { get; set; } = string.Empty;
        public string TreatmentCodeOntologyVersion { get; set; } = string.Empty;
    }
}
