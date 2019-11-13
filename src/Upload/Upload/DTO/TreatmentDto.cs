namespace Upload.DTO
{
    public class TreatmentDto : TreatmentIdDto
    {
        public string TreatmentLocation { get; set; } = string.Empty;
        public string TreatmentCodeOntology { get; set; } = string.Empty;
        public string TreatmentCodeOntologyVersion { get; set; } = string.Empty;
    }
}
