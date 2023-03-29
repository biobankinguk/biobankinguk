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
    public class SampleWriteService : ISampleWriteService
    {
        private readonly ISampleValidationService _sampleValidationService;
        private readonly ApplicationDbContext _db;

        public SampleWriteService(ISampleValidationService sampleValidationService, ApplicationDbContext db)
        {
            _sampleValidationService = sampleValidationService;
            _db = db;
        }

        private static IEnumerable<StagedSample> GetStagedSamplesFromDto(SampleIdDto dto,
            IEnumerable<StagedSample> samples)
            => samples.Where(
                x => x.OrganisationId == dto.OrganisationId &&
                     x.IndividualReferenceId.Equals(dto.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
                     x.Barcode.Equals(dto.Barcode, StringComparison.OrdinalIgnoreCase) &&
                     (x.CollectionName ?? string.Empty).Equals(dto.CollectionName ?? string.Empty, StringComparison.OrdinalIgnoreCase));

        private static IEnumerable<LiveSample> GetLiveSamplesFromDto(SampleIdDto dto,
            IEnumerable<LiveSample> samples)
            => samples.Where(
                x => x.OrganisationId == dto.OrganisationId &&
                     x.IndividualReferenceId.Equals(dto.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
                     x.Barcode.Equals(dto.Barcode, StringComparison.OrdinalIgnoreCase) &&
                     (x.CollectionName ?? string.Empty).Equals(dto.CollectionName ?? string.Empty, StringComparison.OrdinalIgnoreCase));

        public async Task ProcessSamples(IEnumerable<SampleDto> dto)
        {
            var incomingDtos = dto.ToList();
            if (!incomingDtos.Any()) return;

            var organisationId = incomingDtos.FirstOrDefault()?.OrganisationId;
            var allStagedSamples = await _db.StagedSamples
                .Where(x => x.OrganisationId == organisationId)
                .AsNoTracking()
                .ToListAsync();
            var allLiveSamples = await _db.Samples
                .Where(x => x.OrganisationId == organisationId)
                .AsNoTracking()
                .ToListAsync();

            var validationResults = new List<BiobanksValidationResult>();

            foreach (var incomingDto in incomingDtos)
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
                                incomingDto.Barcode
                            })
                        });
                    }

                    throw new AggregateBiobanksValidationException(resultsWithIdentifiers);
                }

                //Get all samples for this DTO, Staged or Live
                var stagedSamples = GetStagedSamplesFromDto(incomingDto, allStagedSamples);
                var liveSamples = GetLiveSamplesFromDto(incomingDto, allLiveSamples);

                //throws on failure, should be allowed to bubble up
                incomingDto.EnsureNewerTimestampThan(stagedSamples);
                incomingDto.EnsureNewerTimestampThan(liveSamples);

                //Check for existing staged record
                var sample = stagedSamples.SingleOrDefault();
                var insert = sample == null;
                sample = sample ?? new StagedSample();

                //try and validate the dto properties and populate the sample with them
                try
                {
                    sample = await _sampleValidationService.ValidateAndPopulateSample(incomingDto, sample);
                }
                catch (AggregateException e)
                {
                    // if problem with sample, add exception, move onto next without insert/update
                    foreach (var exception in e.InnerExceptions)
                    {
                        validationResults.Add(new BiobanksValidationResult
                        {
                            ErrorMessage = exception.Message,
                            RecordIdentifiers = JsonSerializer.Serialize(new
                            {
                                incomingDto.OrganisationId,
                                incomingDto.IndividualReferenceId,
                                incomingDto.Barcode
                            })
                        });
                    }

                    continue;
                }

                // Write the staged sample to the context
                _db.StagedSamples.Update(sample);
            }

            // Commit changes in transaction
            await _db.SaveChangesAsync();

            // if no errors, we're all done!
            if (validationResults.Count <= 0) return;

            throw new AggregateBiobanksValidationException(validationResults);
        }

        public async Task DeleteSamplesIfExists(IEnumerable<SampleIdDto> dto)
        {
            var sampleIdDtos = dto.ToList();
            if (!sampleIdDtos.Any()) return;

            var organisationId = sampleIdDtos.FirstOrDefault()?.OrganisationId;

            var allStagedSamples = await _db.StagedSamples
                .Where(s => s.OrganisationId == organisationId)
                .AsNoTracking()
                .ToListAsync();
            var allLiveSamples = await _db.Samples
                .Where(s => s.OrganisationId == organisationId)
                .AsNoTracking()
                .ToListAsync();

            foreach (var incomingDto in sampleIdDtos)
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
                                incomingDto.Barcode
                            })
                        });
                    }

                    throw new AggregateBiobanksValidationException(resultsWithIdentifiers);
                }

                //Get all samples for this DTO, Staged or Live
                var stagedSamples = GetStagedSamplesFromDto(incomingDto, allStagedSamples).ToList();
                var liveSamples = GetLiveSamplesFromDto(incomingDto, allLiveSamples).ToList();

                //throws on failure, should be allowed to bubble up
                incomingDto.EnsureNewerTimestampThan(stagedSamples);
                incomingDto.EnsureNewerTimestampThan(liveSamples);

                var stagedRecord = stagedSamples.SingleOrDefault();
                var liveRecord = liveSamples.SingleOrDefault();

                //Delete existing staged record if present
                if (stagedRecord != null)
                    _db.StagedSamples.Remove(stagedRecord);

                //Check for existing live record
                if (liveRecord != null)
                    await _db.StagedSampleDeletes.AddAsync(new StagedSampleDelete { Id = liveRecord.Id });
            }

            // Commit all changes in transaction
            await _db.SaveChangesAsync();
        }
    }
}
