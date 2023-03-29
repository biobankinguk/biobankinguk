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
    public class DiagnosisWriteService : IDiagnosisWriteService
    {
        private readonly IDiagnosisValidationService _diagnosisValidationService;
        private readonly ApplicationDbContext _db;

        public DiagnosisWriteService(IDiagnosisValidationService diagnosisValidationService, ApplicationDbContext db)
        {
            _diagnosisValidationService = diagnosisValidationService;
            _db = db;
        }

        private static IEnumerable<StagedDiagnosis> GetStagedDiagnosesFromDto(DiagnosisDto dto,
            IEnumerable<StagedDiagnosis> diagnoses)
            => diagnoses.Where(
                    x => x.OrganisationId == dto.OrganisationId &&
                    x.DateDiagnosed == dto.DateDiagnosed &&
                    x.IndividualReferenceId.Equals(dto.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
                    x.DiagnosisCodeId.Equals(dto.DiagnosisCode, StringComparison.OrdinalIgnoreCase));

        private static IEnumerable<LiveDiagnosis> GetLiveDiagnosesFromDto(DiagnosisDto dto,
            IEnumerable<LiveDiagnosis> diagnoses)
            => diagnoses.Where(
                    x => x.OrganisationId == dto.OrganisationId &&
                    x.DateDiagnosed == dto.DateDiagnosed &&
                    x.IndividualReferenceId.Equals(dto.IndividualReferenceId, StringComparison.OrdinalIgnoreCase) &&
                    x.DiagnosisCodeId.Equals(dto.DiagnosisCode, StringComparison.OrdinalIgnoreCase));

        public async Task ProcessDiagnoses(IEnumerable<DiagnosisDto> dto)
        {
            var diagnosisDtos = dto.ToList();
            if (!diagnosisDtos.Any()) return;

            var organisationId = diagnosisDtos.FirstOrDefault()?.OrganisationId;
            var allStagedDiagnoses = await _db.StagedDiagnoses
                .Where(x => x.OrganisationId == organisationId)
                .AsNoTracking()
                .ToListAsync();
            var allLiveDiagnoses = await _db.Diagnoses
                .Where(x => x.OrganisationId == organisationId)
                .AsNoTracking()
                .ToListAsync();

            var validationResults = new List<BiobanksValidationResult>();

            foreach (var incomingDto in diagnosisDtos)
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
                                incomingDto.DateDiagnosed,
                                incomingDto.DiagnosisCode
                            })
                        });
                    }

                    throw new AggregateBiobanksValidationException(resultsWithIdentifiers);
                }

                //Get all samples for this DTO, Staged or Live
                var stagedDiagnoses = GetStagedDiagnosesFromDto(incomingDto, allStagedDiagnoses);
                var liveDiagnoses = GetLiveDiagnosesFromDto(incomingDto, allLiveDiagnoses);

                //throws on failure, should be allowed to bubble up
                incomingDto.EnsureNewerTimestampThan(stagedDiagnoses);
                incomingDto.EnsureNewerTimestampThan(liveDiagnoses);

                //Check for existing staged record
                var diagnosis = stagedDiagnoses.SingleOrDefault();
                var insert = diagnosis == null;
                diagnosis = diagnosis ?? new StagedDiagnosis();

                //try and validate the dto properties and populate the diagnosis with them
                try
                {
                    diagnosis = (StagedDiagnosis)await _diagnosisValidationService.ValidateAndPopulateDiagnosis(incomingDto, diagnosis);
                }
                catch (AggregateException e)
                {
                    // if problem with diagnosis, add exception, move onto next without insert/update
                    foreach (var exception in e.InnerExceptions)
                    {
                        validationResults.Add(new BiobanksValidationResult
                        {
                            ErrorMessage = exception.Message,
                            RecordIdentifiers = JsonSerializer.Serialize(new
                            {
                                incomingDto.OrganisationId,
                                incomingDto.IndividualReferenceId,
                                incomingDto.DateDiagnosed,
                                incomingDto.DiagnosisCode
                            })
                        });
                    }

                    continue;
                }

                //Write the staged diagnosis to the context
                _db.StagedDiagnoses.Update(diagnosis);
            }

            // Commit changes in transaction
            await _db.SaveChangesAsync();

            // if no errors, we're all done!
            if (validationResults.Count <= 0) return;

            throw new AggregateBiobanksValidationException(validationResults);
        }

        public async Task DeleteDiagnosesIfExists(IEnumerable<DiagnosisDto> dto)
        {
            var diagnosisDtos = dto.ToList();
            if (!diagnosisDtos.Any()) return;

            var organisationId = diagnosisDtos.FirstOrDefault()?.OrganisationId;

            var allStagedDiagnoses = await _db.StagedDiagnoses
                .Where(x => x.OrganisationId == organisationId)
                .AsNoTracking()
                .ToListAsync();
            var allLiveDiagnoses = await _db.Diagnoses
                .Where(x => x.OrganisationId == organisationId)
                .AsNoTracking()
                .ToListAsync();

            foreach (var incomingDto in diagnosisDtos)
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
                                incomingDto.DateDiagnosed,
                                incomingDto.DiagnosisCode
                            })
                        });
                    }

                    throw new AggregateBiobanksValidationException(resultsWithIdentifiers);
                }

                //Get all samples for this DTO, Staged or Live
                var stagedDiagnoses = GetStagedDiagnosesFromDto(incomingDto, allStagedDiagnoses).ToList();
                var liveDiagnoses = GetLiveDiagnosesFromDto(incomingDto, allLiveDiagnoses).ToList();

                //throws on failure, should be allowed to bubble up
                incomingDto.EnsureNewerTimestampThan(stagedDiagnoses);
                incomingDto.EnsureNewerTimestampThan(liveDiagnoses);

                var stagedRecord = stagedDiagnoses.SingleOrDefault();
                var liveRecord = liveDiagnoses.SingleOrDefault();

                //Check for existing staged record
                if (stagedRecord != null)
                {
                    _db.StagedDiagnoses.Remove(stagedRecord);
                }

                //Check for existing live record, and add a staged delete if so
                if (liveRecord != null)
                    await _db.StagedDiagnosisDeletes.AddAsync(new StagedDiagnosisDelete { Id = liveRecord.Id });
            }

            // Commit all changes in transaction
            await _db.SaveChangesAsync();
        }
    }
}
