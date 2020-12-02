﻿namespace Biobanks.SubmissionProcessJob.Dtos
{
    public class TreatmentDto : TreatmentIdDto
    {
        public string TreatmentLocation { get; set; }
        public string TreatmentCodeOntology { get; set; }
        public string TreatmentCodeOntologyVersion { get; set; }
        public string TreatmentCodeOntologyField { get; set; }
    }
}
