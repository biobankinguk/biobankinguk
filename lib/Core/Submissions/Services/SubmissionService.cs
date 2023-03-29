using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Entities.Api;
using Microsoft.EntityFrameworkCore;
using Biobanks.Data;
using LinqKit;
using Core.Submissions.Services.Contracts;
using Core.Submissions.Types;
using System.Text.Json;
using AutoMapper;
using Core.Submissions.Models;
using Core.Submissions.Dto;
using Core.Submissions.Exceptions;

namespace Core.Submissions.Services
{
    /// <inheritdoc />
    public class SubmissionService : ISubmissionService
    {
        // Cloud Services
        private readonly IBlobReadService _blobReadService;
        private readonly IBlobWriteService _blobWriteService;

        // Automapper
        private readonly IMapper _mapper;

        // Entity Validation and Write Services
        private readonly IDiagnosisWriteService _diagnosisWriteService;
        private readonly ISampleWriteService _sampleWriteService;
        private readonly ITreatmentWriteService _treatmentWriteService;
        private readonly IErrorService _errorService;

        private readonly ApplicationDbContext _db;

        /// <inheritdoc />
        public SubmissionService(ApplicationDbContext db, IBlobReadService blobReadService,
            IBlobWriteService blobWriteService, IMapper mapper, IDiagnosisWriteService diagnosisWriteService,
            ISampleWriteService sampleWriteService, ITreatmentWriteService treatmentWriteService,
            IErrorService errorService)
        {
            _db = db;
            _blobReadService = blobReadService;
            _blobWriteService = blobWriteService;
            _mapper = mapper;
            _diagnosisWriteService = diagnosisWriteService;
            _sampleWriteService = sampleWriteService;
            _treatmentWriteService = treatmentWriteService;
            _errorService = errorService;
        }

        public async Task Staging(OperationsQueueItem operationQueueItem)
        {
            const string storageContainer = "submission-payload";

            var blobContents = await _blobReadService.GetObjectFromJsonAsync(storageContainer, operationQueueItem.BlobId);

            // Get The Type From Stored String
            var blobject = JsonSerializer.Deserialize(blobContents, Type.GetType(operationQueueItem.BlobType));
            var subId = operationQueueItem.SubmissionId;

            switch (blobject)
            {
                case List<DiagnosisModel> model:
                    if (!model.Any()) break;

                    switch (operationQueueItem.Operation)
                    {
                        case Operation.Submit:

                            //Transform to a DTO and try to write it
                            var diagnosisDtos = _mapper.Map<IEnumerable<DiagnosisDto>>(model);

                            foreach (var dto in diagnosisDtos)
                            {
                                dto.OrganisationId = operationQueueItem.BiobankId;
                            }

                            try
                            {
                                await _diagnosisWriteService.ProcessDiagnoses(diagnosisDtos);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _errorService.Add(
                                    subId,
                                    operationQueueItem.Operation,
                                    "Diagnosis",
                                    e.ValidationResults,
                                    operationQueueItem.BiobankId);
                            }

                            await ProcessRecords(subId, diagnosisDtos.Count());

                            break;

                        case Operation.Delete:
                            //Transform to a DTO and try to delete it
                            var diagnosisDeletes = _mapper.Map<IEnumerable<DiagnosisDto>>(model);

                            //additional bits automapper won't do
                            foreach (var dto in diagnosisDeletes)
                            {
                                dto.OrganisationId = operationQueueItem.BiobankId;
                            }

                            try
                            {
                                await _diagnosisWriteService.DeleteDiagnosesIfExists(diagnosisDeletes);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _errorService.Add(
                                    subId,
                                    operationQueueItem.Operation,
                                    "Diagnosis",
                                    e.ValidationResults,
                                    operationQueueItem.BiobankId);
                            }

                            await ProcessRecords(subId, diagnosisDeletes.Count());

                            break;
                    }
                    break;

                case List<SampleModel> model:
                    if (!model.Any()) break;

                   // log.LogInformation($"blobject typematch: {model.GetType()}");
                    switch (operationQueueItem.Operation)
                    {
                        case Operation.Submit:
                            //Transform to a DTO and try to write it
                            var sampleDtos = _mapper.Map<IEnumerable<SampleDto>>(model);

                            foreach (var dto in sampleDtos)
                            {
                                dto.OrganisationId = operationQueueItem.BiobankId;
                            }

                            try
                            {
                                await _sampleWriteService.ProcessSamples(sampleDtos);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _errorService.Add(
                                    subId,
                                    operationQueueItem.Operation,
                                    "Sample",
                                    e.ValidationResults,
                                    operationQueueItem.BiobankId);
                            }

                            await ProcessRecords(subId, sampleDtos.Count());

                            break;

                        case Operation.Delete:
                            //Transform to a DTO and try to delete it
                            var sampleDeletes = _mapper.Map<IEnumerable<SampleDto>>(model);

                            //additional bits automapper won't do
                            foreach (var dto in sampleDeletes)
                            {
                                dto.OrganisationId = operationQueueItem.BiobankId;
                            }

                            try
                            {
                                await _sampleWriteService.DeleteSamplesIfExists(sampleDeletes);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _errorService.Add(
                                    subId,
                                    operationQueueItem.Operation,
                                    "Sample",
                                    e.ValidationResults,
                                    operationQueueItem.BiobankId);
                            }

                            await ProcessRecords(subId, sampleDeletes.Count());

                            break;
                    }
                    break;

                case List<TreatmentModel> model:
                    if (!model.Any()) break;

                  //  log.LogInformation($"blobject typematch: {model.GetType()}");
                    switch (operationQueueItem.Operation)
                    {
                        case Operation.Submit:
                            //Transform to a DTO and try to write it
                            var treatmentDtos = _mapper.Map<IEnumerable<TreatmentDto>>(model);

                            foreach (var dto in treatmentDtos)
                            {
                                dto.OrganisationId = operationQueueItem.BiobankId;
                            }

                            try
                            {
                                await _treatmentWriteService.ProcessTreatments(treatmentDtos);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _errorService.Add(
                                    subId,
                                    operationQueueItem.Operation,
                                    "Treatment",
                                    e.ValidationResults,
                                    operationQueueItem.BiobankId);
                            }

                            await ProcessRecords(subId, treatmentDtos.Count());

                            break;

                        case Operation.Delete:
                            //Transform to a DTO and try to delete it
                            var treatmentDeletes = _mapper.Map<IEnumerable<TreatmentDto>>(model);

                            //additional bits automapper won't do
                            foreach (var dto in treatmentDeletes)
                            {
                                dto.OrganisationId = operationQueueItem.BiobankId;
                            }

                            try
                            {
                                await _treatmentWriteService.DeleteTreatmentsIfExists(treatmentDeletes);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _errorService.Add(
                                    subId,
                                    operationQueueItem.Operation,
                                    "Treatment",
                                    e.ValidationResults,
                                    operationQueueItem.BiobankId);
                            }

                            await ProcessRecords(subId, treatmentDeletes.Count());

                            break;
                    }
                    break;
            }

            await _blobWriteService.DeleteAsync(storageContainer, operationQueueItem.BlobId);
        }

        /// <inheritdoc />
        public async Task<Submission> Get(int submissionId)
            => await _db.Submissions
                .AsNoTracking()
                .Include(x => x.Status)
                .Include(x => x.Errors)
                .SingleOrDefaultAsync(x => x.Id == submissionId);

        /// <inheritdoc />
        public async Task<(int total, IEnumerable<Submission> submissions)>
            List(int biobankId, SubmissionPaginationParams paging)
        {
            // we're gonna build up conditional stuff on this query, so store a basic one for now
            var query = _db.Submissions.AsNoTracking();

            var predicate = PredicateBuilder.New<Submission>(x => x.BiobankId == biobankId); //always filter on bb id

            DateTime? since = null; //so we can abuse since mechanics WITHOUT modifying the paging object

            if (paging.N > 0 && paging.Since == null) //since precedes n
            {
                //last n commits essentially abuses the since logic:
                //we set since to the earliest timestamp of the nth last commit
                //and then proceed like we would for a regular since query

                //get the timestamp of the nth most recent commit
                var nthCommitTimeStamp = (await query
                        .Where(predicate.And(y => y.Status.Value == Statuses.Committed))
                        .OrderByDescending(x => x.SubmissionTimestamp)
                        .Select(x => new { x.StatusChangeTimestamp, x.SubmissionTimestamp })
                        .Distinct()
                        .Take(paging.N)
                        .ToListAsync()) //Execute SQL here so the ordering works how we expect?
                    .LastOrDefault()
                    ?.StatusChangeTimestamp;

                //set since to the earliest submission timestamp in that commit
                if (nthCommitTimeStamp != null)
                {
                    since = await query
                    .Where(x => x.StatusChangeTimestamp == nthCommitTimeStamp)
                        .Select(x => x.SubmissionTimestamp)
                        .FirstOrDefaultAsync();
                }

                //if we had no success, "since" will be set to DateTime's default initial value
                //which is kinda useless to us
                //so we should drop it
                if (since == new DateTime()) since = null;
            }

            if (paging.Since != null) since = paging.Since;

            if (since != null) predicate = predicate.And(x => x.SubmissionTimestamp > since);

            query = query.Where(predicate); //add the filter permanently now

            return (await query.CountAsync(),
                await query
                    .OrderByDescending(x => x.SubmissionTimestamp)
                    .Include(x => x.Status)
                    .Include(x => x.Errors)
                    .Skip(paging.Offset)
                    .Take(paging.Limit)
                    .ToListAsync());
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Submission>> ListSubmissionsInProgress(int biobankId)
            => await _db.Submissions.Where(s =>
                s.BiobankId == biobankId
                && s.Status.Value == Statuses.Open
                && s.RecordsProcessed != s.TotalRecords)
                .AsNoTracking()
                .Include(x => x.Status)
                .ToListAsync();

        /// <inheritdoc />
        public async Task<Submission> CreateSubmission(int totalRecords, int biobankId)
        {
            var status = await _db.Statuses
                .Where(x => x.Value == Statuses.Open)
                .SingleAsync();

            var sub = new Submission
            {
                BiobankId = biobankId,
                TotalRecords = totalRecords,
                Status = status
            };

            await _db.Submissions.AddAsync(sub);

            await _db.SaveChangesAsync();

            return sub;
        }

        /// <inheritdoc />
        public async Task DeleteSubmission(int submissionId)
        {
            var submission = await _db.Submissions.SingleAsync(s => s.Id == submissionId);
            _db.Remove(submission);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task ProcessRecords(int submissionId, int n)
        {
            var sub = await _db.Submissions.Where(x => x.Id == submissionId).SingleOrDefaultAsync();

            if (sub == null) throw new KeyNotFoundException();

            var newTotalRecordsProcessed = sub.RecordsProcessed + n;
            sub.RecordsProcessed = newTotalRecordsProcessed <= sub.TotalRecords ? newTotalRecordsProcessed : sub.TotalRecords;
            sub.StatusChangeTimestamp = DateTime.Now;

            await _db.SaveChangesAsync();
        }
    }
}