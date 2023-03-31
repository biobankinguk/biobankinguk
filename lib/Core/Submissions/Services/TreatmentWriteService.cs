using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.Api;
using Biobanks.Submissions.Dto;
using Biobanks.Submissions.Exceptions;
using Biobanks.Submissions.Extensions;
using Biobanks.Submissions.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Services
{
    public class TreatmentWriteService : ITreatmentWriteService
    {
        private readonly ITreatmentValidationService _treatmentValidationService;
        private readonly ApplicationDbContext _db;

        public TreatmentWriteService(ITreatmentValidationService treatmentValidationService, ApplicationDbContext db)
        {
            _treatmentValidationService = treatmentValidationService;
            _db = db;
        }

        private static IEnumerable<StagedTreatment> GetStagedTreatmentsFromDto(TreatmentIdDto dto,
            IEnumerable<StagedTreatment> treatments)
            => treatments.Where(
                x => x.OrganisationId == dto.OrganisationId &&
                x.DateTreated == dto.DateTreated &&
                x.IndividualReferenceId.Equals(dto.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
                x.TreatmentCodeId.Equals(dto.TreatmentCode, StringComparison.OrdinalIgnoreCase));

        private static IEnumerable<LiveTreatment> GetLiveTreatmentsFromDto(TreatmentIdDto dto,
            IEnumerable<LiveTreatment> treatments)
            => treatments.Where(
                x => x.OrganisationId == dto.OrganisationId &&
                x.DateTreated == dto.DateTreated &&
                x.IndividualReferenceId.Equals(dto.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
                x.TreatmentCodeId.Equals(dto.TreatmentCode, StringComparison.OrdinalIgnoreCase));

        public async Task ProcessTreatments(IEnumerable<TreatmentDto> dto)
        {
            var treatmentDtos = dto.ToList();
            if (!treatmentDtos.Any()) return;

            var organisationId = treatmentDtos.FirstOrDefault()?.OrganisationId;
            var allStagedTreatments = await _db.StagedTreatments
                .Where(x => x.OrganisationId == organisationId)
                .AsNoTracking()
                .ToListAsync();
            var allLiveTreatments = await _db.Treatments
                .Where(x => x.OrganisationId == organisationId)
                .AsNoTracking()
                .ToListAsync();

            var validationResults = new List<BiobanksValidationResult>();

            foreach (var incomingDto in treatmentDtos)
            {
                //controller should have done this, but: model validate the dto
                var vcontext = new ValidationContext(dto);
                var results = new List<ValidationResult>();
                var resultsWithIdentifiers = new List<BiobanksValidationResult>();
                if (!Validator.TryValidateObject(dto, vcontext, results, true))
                {
                    foreach (var result in results)
                    {
                        resultsWithIdentifiers.Add(new BiobanksValidationResult
                        {
                            ErrorMessage = result.ErrorMessage,
                            RecordIdentifiers = JsonSerializer.Serialize(new
                            {
                                incomingDto.OrganisationId,
                                incomingDto.IndividualReferenceId,
                                incomingDto.DateTreated,
                                incomingDto.TreatmentCode
                            })
                        });
                    }

                    throw new AggregateBiobanksValidationException(resultsWithIdentifiers);
                }

                //Get all samples for this DTO, Staged or Live
                var stagedTreatments = GetStagedTreatmentsFromDto(incomingDto, allStagedTreatments);
                var liveTreatments = GetLiveTreatmentsFromDto(incomingDto, allLiveTreatments);

                //throws on failure, should be allowed to bubble up
                incomingDto.EnsureNewerTimestampThan(stagedTreatments);
                incomingDto.EnsureNewerTimestampThan(liveTreatments);

                //Check for existing staged record
                var treatment = stagedTreatments.SingleOrDefault();
                var insert = treatment == null;
                treatment = treatment ?? new StagedTreatment();

                //try and validate the dto properties and populate the treatment with them
                try
                {
                    treatment = await _treatmentValidationService.ValidateAndPopulateTreatment(incomingDto, treatment);
                }
                catch (AggregateException e)
                {
                    // if problem with treatment, add exception, move onto next without insert/update
                    foreach (var exception in e.InnerExceptions)
                    {
                        validationResults.Add(new BiobanksValidationResult
                        {
                            ErrorMessage = exception.Message,
                            RecordIdentifiers = JsonSerializer.Serialize(new
                            {
                                incomingDto.OrganisationId,
                                incomingDto.IndividualReferenceId,
                                incomingDto.DateTreated,
                                incomingDto.TreatmentCode
                            })
                        });
                    }

                    continue;
                }

                //Write the staged treatment to the context
                _db.StagedTreatments.Update(treatment);
            }

            // Commit changes in transaction
            await _db.SaveChangesAsync();

            // if no errors, we're all done!
            if (validationResults.Count <= 0) return;

            throw new AggregateBiobanksValidationException(validationResults);
        }

        public async Task DeleteTreatmentsIfExists(IEnumerable<TreatmentIdDto> dto)
        {
            var treatmentIdDtos = dto.ToList();
            if (!treatmentIdDtos.Any()) return;

            var organisationId = treatmentIdDtos.FirstOrDefault()?.OrganisationId;

            var allStagedTreatments = await _db.StagedTreatments
                .Where(x => x.OrganisationId == organisationId)
                .AsNoTracking()
                .ToListAsync();
            var allLiveTreatments = await _db.Treatments
                .Where(x => x.OrganisationId == organisationId)
                .AsNoTracking()
                .ToListAsync();

            foreach (var incomingDto in treatmentIdDtos)
            {
                //controller should have done this, but: model validate the dto
                var vcontext = new ValidationContext(dto);
                var results = new List<ValidationResult>();
                var resultsWithIdentifiers = new List<BiobanksValidationResult>();
                if (!Validator.TryValidateObject(dto, vcontext, results, true))
                {
                    foreach (var result in results)
                    {
                        resultsWithIdentifiers.Add(new BiobanksValidationResult
                        {
                            ErrorMessage = result.ErrorMessage,
                            RecordIdentifiers = JsonSerializer.Serialize(new
                            {
                                incomingDto.OrganisationId,
                                incomingDto.IndividualReferenceId,
                                incomingDto.DateTreated,
                                incomingDto.TreatmentCode
                            })
                        });
                    }

                    throw new AggregateBiobanksValidationException(resultsWithIdentifiers);
                }

                //Get all samples for this DTO, Staged or Live
                var stagedTreatments = GetStagedTreatmentsFromDto(incomingDto, allStagedTreatments).ToList();
                var liveTreatments = GetLiveTreatmentsFromDto(incomingDto, allLiveTreatments).ToList();

                //throws on failure, should be allowed to bubble up
                incomingDto.EnsureNewerTimestampThan(stagedTreatments);
                incomingDto.EnsureNewerTimestampThan(liveTreatments);

                var stagedRecord = stagedTreatments.SingleOrDefault();
                var liveRecord = liveTreatments.SingleOrDefault();

                //Check for existing staged record
                if (stagedRecord != null)
                    _db.StagedTreatments.Remove(stagedRecord);

                //Check for existing staged record
                if (liveRecord != null)
                    await _db.StagedTreatmentDeletes.AddAsync(new StagedTreatmentDelete() { Id = liveRecord.Id });
            }

            // Commit all changes in transaction
            await _db.SaveChangesAsync();
        }
    }
}
