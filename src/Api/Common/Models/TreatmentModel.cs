namespace Biobanks.Common.Models
{
    public class TreatmentModel : TreatmentIdModel
    {
        public string TreatmentLocation { get; set; }
        public string TreatmentCodeOntology { get; set; }
        public string TreatmentCodeOntologyVersion { get; set; }
    }
}
