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
    public class TreatmentValidationService : ITreatmentValidationService
    {
        private readonly IReferenceDataReadService _refDataReadService;

        public TreatmentValidationService(IReferenceDataReadService refDataReadService)
        {
            _refDataReadService = refDataReadService;
        }

        public async Task<StagedTreatment> ValidateAndPopulateTreatment(TreatmentDto dto, StagedTreatment treatment = null)
        {
            treatment = treatment ?? new StagedTreatment();
            treatment.OrganisationId = dto.OrganisationId;

            var exceptions = new List<ValidationException>();

            try { treatment = ValidateFutureDate(dto, treatment); }
            catch (ValidationException e) { exceptions.Add(e); }
            try { treatment = await ValidateTreatmentCode(dto, treatment); }
            catch (ValidationException e) { exceptions.Add(e); }
            try { treatment = await ValidateTreatmentLocation(dto, treatment); }
            catch (ValidationException e) { exceptions.Add(e); }
            try { treatment = await ValidateTreatmentCodeOntologyVersion(dto, treatment); }
            catch (ValidationException e) { exceptions.Add(e); }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            //system properties
            treatment.SubmissionTimestamp = dto.SubmissionTimestamp;
            treatment.IndividualReferenceId = dto.IndividualReferenceId;

            return treatment;
        }

        private StagedTreatment ValidateFutureDate(TreatmentDto dto, StagedTreatment treatment)
        {
            // Check Date Isn't In The Future
            if (dto.DateTreated > dto.SubmissionTimestamp)
            {
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.DateInFuture(dto.DateTreated, dto.IndividualReferenceId)
                    ),
                    null, null
                );
            }

            // Map Across Valid Date
            treatment.DateTreated = dto.DateTreated;

            return treatment;
        }

        private async Task<StagedTreatment> ValidateTreatmentLocation(TreatmentDto dto, StagedTreatment treatment)
        {
            var result = await _refDataReadService.GetTreatmentLocation(dto.TreatmentLocation);

            if (result == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.TreatmentLocation(dto.TreatmentLocation, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.TreatmentLocation) }),
                    null, null);

            treatment.TreatmentLocationId = result.Id;
            return treatment;
        }

        private async Task<StagedTreatment> ValidateTreatmentCodeOntologyVersion(TreatmentDto dto, StagedTreatment treatment)
        {
            var ontology = await _refDataReadService.GetOntology(dto.TreatmentCodeOntology);

            if (ontology == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.TreatmentCodeOntology(dto.TreatmentCodeOntology, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.TreatmentCodeOntology) }),
                    null, null);

            var ontologyVersion =
                ontology.OntologyVersions.FirstOrDefault(x => x.Value == dto.TreatmentCodeOntologyVersion);

            if (ontologyVersion == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.TreatmentCodeOntologyVersion(dto.TreatmentCodeOntologyVersion, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.TreatmentCodeOntologyVersion) }),
                    null, null);

            treatment.TreatmentCodeOntologyVersionId = ontologyVersion.Id;
            return treatment;
        }

        private async Task<StagedTreatment> ValidateTreatmentCode(TreatmentDto dto, StagedTreatment treatment)
        {
            // TODO Change this to use generic ontology lookup service in future
            var result = await _refDataReadService.GetSnomedTreatment(dto.TreatmentCode, dto.TreatmentCodeOntologyField);

            if (result == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.SnomedTreatment(dto.TreatmentCode, dto.TreatmentCodeOntologyField, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.TreatmentCode) }),
                    null, null);

            treatment.TreatmentCodeId = result.Id;
            return treatment;
        }
    }
}
