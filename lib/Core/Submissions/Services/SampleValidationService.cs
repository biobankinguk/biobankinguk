using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Biobanks.Data.Entities.Api;
using Biobanks.Submissions.Config;
using Biobanks.Submissions.Dto;
using Biobanks.Submissions.Models.OptionsModels;
using Biobanks.Submissions.Services.Contracts;
using Biobanks.Submissions.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Biobanks.Submissions.Services
{
    public class SampleValidationService : ISampleValidationService
    {
        private readonly IReferenceDataReadService _refDataReadService;
        private readonly ILogger<SampleValidationService> _logger;
        private readonly StorageTemperatureLegacyModel _storageTemperatureLegacy;
        private readonly MaterialTypesLegacyModel _materialTypesLegacy;

        public SampleValidationService(
            IReferenceDataReadService refDataReadService, 
            IOptions<StorageTemperatureLegacyModel> storageTempLegacy, 
            IOptions<MaterialTypesLegacyModel> materialTypeLegacy,
            ILogger<SampleValidationService> logger)
        {
            _refDataReadService = refDataReadService;
            _storageTemperatureLegacy = storageTempLegacy.Value;
            _materialTypesLegacy = materialTypeLegacy.Value;
            _logger = logger;
        }

        public async Task<StagedSample> ValidateAndPopulateSample(SampleDto dto, StagedSample sample = null)
        {
            sample = sample ?? new StagedSample();
            sample.OrganisationId = dto.OrganisationId;

            //Sadly because of EF we can't do these in parallel :( but we still need to aggregate exceptions
            var exceptions = new List<ValidationException>();

            try { sample = ValidateFutureDate(dto, sample); }
            catch (ValidationException e) { exceptions.Add(e); }
            try { sample = await ValidateMaterialType(dto, sample); }
            catch (ValidationException e) { exceptions.Add(e); }
            try { sample = await ValidateStorageTemperature(dto, sample); }
            catch (ValidationException e) { exceptions.Add(e); }
            try { sample = await ValidatePreservationType(dto, sample); }
            catch (ValidationException e) { exceptions.Add(e); }
            try { sample = await ValidateSampleContentMethod(dto, sample); }
            catch (ValidationException e) { exceptions.Add(e); }
            try { sample = await ValidateExtractionSite(dto, sample); }
            catch (ValidationException e) { exceptions.Add(e); }
            try { sample = await ValidateExtractionProcedure(dto, sample); }
            catch (ValidationException e) { exceptions.Add(e); }
            try { sample = await ValidateSampleContent(dto, sample); }
            catch (ValidationException e) { exceptions.Add(e); }
            try { sample = await ValidateSex(dto, sample); }
            catch (ValidationException e) { exceptions.Add(e); }
            try { sample = ValidateAgeAndYearOfBirth(dto, sample); }
            catch (ValidationException e) { exceptions.Add(e); }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            //value properties (don't need to await)
            sample.IndividualReferenceId = dto.IndividualReferenceId;
            sample.Barcode = dto.Barcode;
            sample.SubmissionTimestamp = dto.SubmissionTimestamp;
            sample.CollectionName = dto.CollectionName;

            return sample;
        }

        private StagedSample ValidateFutureDate(SampleDto dto, StagedSample sample)
        {
            // Check Date Isn't In The Future
            if (dto.DateCreated > dto.SubmissionTimestamp)
            {
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.DateInFuture(dto.DateCreated, dto.IndividualReferenceId)
                    ),
                    null, null
                );
            }

            // Map Across Valid Date
            sample.DateCreated = dto.DateCreated;

            return sample;
        }

        private StagedSample ValidateAgeAndYearOfBirth(SampleDto dto, StagedSample sample)
        {
            // if neither YearOfBirth nor AgeAtDonation are provided, throw an error
            if (string.IsNullOrEmpty(dto.AgeAtDonation) && dto.YearOfBirth == null)
                throw new ValidationException(
                    new ValidationResult(
                        $"At least one of {nameof(dto.AgeAtDonation)} or {nameof(dto.YearOfBirth)} must be provided.",
                        new List<string> { nameof(dto.AgeAtDonation), nameof(dto.YearOfBirth) }),
                    null, null);

            // if only AgeAtDonation is provided, use it and return
            if (dto.YearOfBirth == null)
            {
                sample.AgeAtDonation = dto.AgeAtDonation;
                return sample;
            }

            // if only YearOfBirth is specified, calculate confident AgeAtDonation and return
            if (string.IsNullOrEmpty(dto.AgeAtDonation))
            {
                sample.YearOfBirth = (int)dto.YearOfBirth;
                // Creates an AgeAtDonation Timespan value from date created - 01/01/YearOfBirth
                /* If DateCreated is in the same year as YearOfBirth - then it will be positive duration
                   Otherwise it will be negative (fetus)   */
                var ageAtDonation = dto.DateCreated - new DateTime((int)sample.YearOfBirth, 01, 01);

                // Converts Timespan value to Iso Duration
                sample.AgeAtDonation = XmlConvert.ToString(ageAtDonation);

                return sample;
            }

            // otherwise, check that the provided YearOfBirth and AgeAtDonation add up

            var ageAtDonationTs = XmlConvert.ToTimeSpan(dto.AgeAtDonation);

            var maxTs = dto.DateCreated - new DateTime((int)dto.YearOfBirth, 01, 01);
            var minTs = dto.DateCreated - new DateTime((int)dto.YearOfBirth, 12, 31);

            // if AgeAtDonation doesnt fall within the bounds of DateCreated - YearOfBirth, throw an error
            if (ageAtDonationTs <= minTs || ageAtDonationTs >= maxTs)
            {
                throw new ValidationException(
                    new ValidationResult(
                       $"If both {nameof(dto.AgeAtDonation)} and {nameof(dto.YearOfBirth)} are provided, the {nameof(dto.AgeAtDonation)} timespan value must fall within the minimum/maximum timepsans based on the difference between {nameof(dto.DateCreated)} and {nameof(dto.YearOfBirth)}.",
                        new List<string> { nameof(dto.AgeAtDonation), nameof(dto.YearOfBirth) }),
                    null, null);
            }

            // if AgeAtDonation does fall within the bounds, use the original values and return
            sample.AgeAtDonation = dto.AgeAtDonation;
            sample.YearOfBirth = (int)dto.YearOfBirth;
            return sample;
        }

        private async Task<StagedSample> ValidateSampleContent(SampleDto dto, StagedSample sample)
        {
            // regardless of material type, if both props are empty, it is valid
            if (string.IsNullOrWhiteSpace(dto.SampleContentMethod) && string.IsNullOrWhiteSpace(dto.SampleContent))
                return sample;

            //check if extracted sample
            var mt = await _refDataReadService.GetMaterialType(dto.MaterialType);
            if (!(mt?.MaterialTypeGroups.Any(x => x.Value == MaterialTypeGroups.ExtractedSample) ?? false))
                return sample; //not invalid, but irrelevant, so no value

            // Validate SNOMED-CT term
            // TODO Change this to use generic ontology lookup service in future
            var result = await _refDataReadService.GetSnomedDiagnosis(dto.SampleContent, dto.SampleContentOntologyField);

            if (result == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.SnomedSampleContent(dto.SampleContent, dto.SampleContentOntologyField, dto.Barcode, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.SampleContent) }),
                    null, null);

            sample.SampleContentId = result.Id;
            return sample;
        }

        private async Task<StagedSample> ValidateExtractionProcedure(SampleDto dto, StagedSample sample)
        {
            var mt = (await _refDataReadService.ListMaterialTypes()).FirstOrDefault(x => x.Value == dto.MaterialType);
            var ep = await _refDataReadService.GetSnomedExtractionProcedure(dto.ExtractionProcedure, dto.ExtractionProcedureOntologyField);

            // Only Validate If MaterialType Has Explicit ExtractionProcedures
            if (mt == null || !mt.ExtractionProcedures.Any())
            {
                return sample;
            }

            // Invalid ExtractionProcedures
            if (ep == null)
            {
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.SnomedExtractionProcedure(dto.ExtractionProcedure, dto.ExtractionProcedureOntologyField, dto.Barcode, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.ExtractionProcedure) }),
                    null, null);
            }

            // Check MaterialType is valid for given Extraction Procedure
            if (!ep.MaterialTypes.Select(x => x.Id).Contains(mt.Id))
            {
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.ExtractionProcedureMaterialTypeMismatch(dto.ExtractionProcedure, dto.MaterialType, dto.Barcode, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.ExtractionProcedure), nameof(dto.MaterialType) }),
                    null, null);
            }

            // Validated ExtractionProcedure
            sample.ExtractionProcedureId = ep.Id;

            return sample;
        }

        private async Task<StagedSample> ValidateExtractionSite(SampleDto dto, StagedSample sample)
        {
            //check if tissue sample
            var mt = await _refDataReadService.GetMaterialType(dto.MaterialType);

            if (!(mt?.MaterialTypeGroups.Any(x => x.Value == MaterialTypeGroups.TissueSample) ?? false))
                return sample; //not invalid, but irrelevant, so no value

            // Validate SNOMED-CT term
            // TODO Change this to use generic ontology lookup service in future
            var result = await _refDataReadService.GetSnomedBodyOrgan(dto.ExtractionSite, dto.ExtractionSiteOntologyField);

            if (result == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.SnomedBodyOrgan(dto.ExtractionSite, dto.ExtractionSiteOntologyField, dto.Barcode, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.ExtractionSite) }),
                    null, null);

            sample.ExtractionSiteId = result.Id;

            var ontology = await _refDataReadService.GetOntology(dto.ExtractionSiteOntology);

            if (ontology == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.ExtractionSiteOntology(dto.ExtractionSiteOntology, dto.Barcode, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.ExtractionSiteOntology) }),
                    null, null);

            var ontologyVersion =
                ontology.OntologyVersions.FirstOrDefault(x => x.Value == dto.ExtractionSiteOntologyVersion);

            if (ontologyVersion == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.ExtractionSiteOntologyVersion(dto.ExtractionSiteOntologyVersion, dto.Barcode, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.ExtractionSiteOntologyVersion) }),
                    null, null);

            sample.ExtractionSiteOntologyVersionId = ontologyVersion.Id;
            return sample;
        }

        private async Task<StagedSample> ValidateSampleContentMethod(SampleDto dto, StagedSample sample)
        {
            // regardless of material type, if both props are empty, it is valid
            if (string.IsNullOrWhiteSpace(dto.SampleContentMethod) && string.IsNullOrWhiteSpace(dto.SampleContent))
                return sample;

            //check if extracted sample
            var mt = await _refDataReadService.GetMaterialType(dto.MaterialType);
            if (!(mt?.MaterialTypeGroups.Any(x => x.Value == MaterialTypeGroups.ExtractedSample) ?? false))
                return sample; //not invalid, but irrelevant, so no value

            var result = await _refDataReadService.GetSampleContentMethod(dto.SampleContentMethod);

            if (result == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.SampleContentMethod(dto.SampleContentMethod, dto.SampleContentOntologyField, dto.Barcode, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.SampleContentMethod) }),
                    null, null);

            sample.SampleContentMethodId = result.Id;
            return sample;
        }

        private async Task<StagedSample> ValidateStorageTemperature(SampleDto dto, StagedSample sample)
        {
            foreach (var obj in _storageTemperatureLegacy?.ListOfMappings ?? new ())
            {
                if (obj.Old.StorageTemperature == dto.StorageTemperature)
                {
                    dto.StorageTemperature = obj.New.StorageTemperature;
                    _logger.LogInformation($"The given storage temperature was mapped to {obj.New.StorageTemperature}");
                    var preservationType = await _refDataReadService.GetPreservationType(obj.New.PreservationType);
                    sample.PreservationTypeId = preservationType.Id;
                }
            }
            var result = await _refDataReadService.GetStorageTemperature(dto.StorageTemperature);

            if (result == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.StorageTemperature(dto.StorageTemperature, dto.Barcode, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.StorageTemperature) }),
                    null, null);

            sample.StorageTemperatureId = result.Id;

            return sample;
        }

        private async Task<StagedSample> ValidatePreservationType(SampleDto dto, StagedSample sample)
        {
            if (string.IsNullOrEmpty(dto.PreservationType))
                return sample;

            var result = await _refDataReadService.GetPreservationType(dto.PreservationType);

            if (result is null)
            {
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.PreservationType(dto.PreservationType, dto.Barcode, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.PreservationType) }),
                    null, null);
            }

            if (sample.StorageTemperatureId != result.StorageTemperatureId)
            {
                throw new ValidationException(
                    new ValidationResult(
                        $"The provided {nameof(dto.PreservationType)} is not applicable for the provided {nameof(dto.StorageTemperature)}.",
                        new List<string> { nameof(dto.PreservationType), nameof(dto.StorageTemperature) }),
                    null, null);
            }

            sample.PreservationTypeId = result.Id;

            return sample;
        }

        private async Task<StagedSample> ValidateSex(SampleDto dto, StagedSample sample)
        {
            if (string.IsNullOrEmpty(dto.Sex))
                return sample;

            var result = await _refDataReadService.GetSex(dto.Sex);

            if (result == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.Sex(dto.Sex, dto.Barcode, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.Sex) }),
                    null, null);

            sample.SexId = result.Id;
            return sample;
        }

        private async Task<StagedSample> ValidateMaterialType(SampleDto dto, StagedSample sample)
        {
            var legacyMapping = _materialTypesLegacy?.ListOfMappings.FirstOrDefault(x => x.Old.MaterialType == dto.MaterialType);

            if (legacyMapping != null)
            {
                dto.MaterialType = legacyMapping.New.MaterialType;
                dto.ExtractionProcedure = legacyMapping.New.ExtractionProcedure;
                dto.ExtractionProcedureOntologyField = legacyMapping.New.ExtractionProcedureOntologyField;
            }

            var result = await _refDataReadService.GetMaterialType(dto.MaterialType);

            if (result == null)
                throw new ValidationException(
                    new ValidationResult(
                        ValidationErrors.MaterialType(dto.MaterialType, dto.Barcode, dto.IndividualReferenceId),
                        new List<string> { nameof(dto.MaterialType) }),
                    null, null);

            sample.MaterialTypeId = result.Id;

            return sample;
        }
    }
}
