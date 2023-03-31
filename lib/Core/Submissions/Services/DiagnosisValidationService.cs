using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities.Api;
using Biobanks.Submissions.Config;
using Biobanks.Submissions.Dto;
using Biobanks.Submissions.Services.Contracts;

namespace Biobanks.Submissions.Services
{
    public class DiagnosisValidationService : IDiagnosisValidationService
    {
        private readonly IReferenceDataReadService _refDataReadService;

        public DiagnosisValidationService(
            IReferenceDataReadService refDataReadService)
        {
            _refDataReadService = refDataReadService;
        }

        public async Task<Diagnosis> ValidateAndPopulateDiagnosis(DiagnosisDto dto, StagedDiagnosis diagnosis = null)
        {
            diagnosis ??= new StagedDiagnosis();
            diagnosis.OrganisationId = dto.OrganisationId;

            var exceptions = new List<ValidationException>();

            try { diagnosis = ValidateFutureDate(dto, diagnosis); }
            catch (ValidationException e) { exceptions.Add(e); }
            try { diagnosis = await ValidateDiagnosisCode(dto, diagnosis); }
            catch (ValidationException e) { exceptions.Add(e); }
            try { diagnosis = await ValidateDiagnosisCodeOntologyVersion(dto, diagnosis); }
            catch (ValidationException e) { exceptions.Add(e); }

            if (exceptions.Any()) throw new AggregateException(exceptions); //throw the aggregate one

            // System Properties - So No Need To Validate
            diagnosis.SubmissionTimestamp = dto.SubmissionTimestamp;
            diagnosis.IndividualReferenceId = dto.IndividualReferenceId;

            return diagnosis;
        }

        private StagedDiagnosis ValidateFutureDate(DiagnosisDto dto, StagedDiagnosis diagnosis)
        {
            // Check Date Isn't In The Future
            if (dto.DateDiagnosed > dto.SubmissionTimestamp)
            {
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.DateInFuture(dto.DateDiagnosed, dto.IndividualReferenceId)
                    ),
                    null, null
                );
            }

            // Map Across Valid Date
            diagnosis.DateDiagnosed = dto.DateDiagnosed;

            return diagnosis;
        }

        private async Task<StagedDiagnosis> ValidateDiagnosisCode(DiagnosisDto dto, StagedDiagnosis diagnosis)
        {
            // TODO Change this to use generic ontology lookup service in future
            var result = await _refDataReadService.GetSnomedDiagnosis(dto.DiagnosisCode, dto.DiagnosisCodeOntologyField);

            if (result == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.SnomedDiagnosis(dto.DiagnosisCode, dto.DiagnosisCodeOntologyField, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.DiagnosisCode) }),
                    null, null);

            diagnosis.DiagnosisCodeId = result.Id;
            return diagnosis;
        }

        private async Task<StagedDiagnosis> ValidateDiagnosisCodeOntologyVersion(DiagnosisDto dto, StagedDiagnosis diagnosis)
        {
            var ontology = await _refDataReadService.GetOntology(dto.DiagnosisCodeOntology);

            if (ontology == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.DiagnosisCodeOntology(dto.DiagnosisCodeOntology, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.DiagnosisCodeOntology) }),
                    null, null);

            var ontologyVersion =
                ontology.OntologyVersions.FirstOrDefault(x => x.Value == dto.DiagnosisCodeOntologyVersion);

            if (ontologyVersion == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.DiagnosisCodeOntologyVersion(dto.DiagnosisCodeOntologyVersion, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.DiagnosisCodeOntologyVersion) }),
                    null, null);

            diagnosis.DiagnosisCodeOntologyVersionId = ontologyVersion.Id;
            return diagnosis;
        }
    }
}
