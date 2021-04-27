using AutoMapper;
using Biobanks.Submissions.Core.Dto;
using Biobanks.Submissions.Core.Exceptions;
using Biobanks.Submissions.Core.Models;
using Biobanks.Submissions.Core.Services.Contracts;
using Biobanks.Submissions.Core.Types;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Biobanks.Submissions.AzFunctions
{
    public class SubmissionStagingFunction
    {
        // Cloud Services
        private readonly IBlobReadService _blobReader;
        private readonly IBlobWriteService _blobWriter;
        private readonly IQueueWriteService _queueWriter;

        // Automapper
        private readonly IMapper _mapper;

        // Submission Status
        private readonly ISubmissionService _submissions;

        // Entity Validation and Write Services
        private readonly IDiagnosisWriteService _diagnoses;
        private readonly ISampleWriteService _samples;
        private readonly ITreatmentWriteService _treatments;
        private readonly IErrorService _errors;

        public SubmissionStagingFunction(
            IBlobReadService blobReader,
            IBlobWriteService blobWriter,
            IQueueWriteService queueWriter,
            ISubmissionService submissions,
            IDiagnosisWriteService diagnoses,
            ISampleWriteService samples,
            ITreatmentWriteService treatments,
            IErrorService errors,
            IMapper mapper)
        {
            _blobReader = blobReader;
            _blobWriter = blobWriter;
            _queueWriter = queueWriter;
            _submissions = submissions;
            _diagnoses = diagnoses;
            _samples = samples;
            _treatments = treatments;
            _errors = errors;
            _mapper = mapper;
        }

        [Function("Submissions_Staging")]
        public async Task Run(
            [QueueTrigger("operations", Connection = "WorkerStorage")] string messageBody,
            FunctionContext context, // out of process context
                                     // QueueTrigger metadata
            string id,
            string popReceipt)
        {
            var log = context.GetLogger("Submissions_Staging");

            const string storageContainer = "submission-payload";
            var message = JsonSerializer.Deserialize<OperationsQueueItem>(messageBody);
            var blobContents = await _blobReader.GetObjectFromJsonAsync(storageContainer, message.BlobId);
            var biobankId = message.BiobankId;

            // Get The Type From Stored String
            var blobType = Type.GetType(message.BlobType);

            log.LogInformation($"BlobType: {message.BlobType}");
            log.LogInformation(blobContents);

            var blobject = JsonSerializer.Deserialize(blobContents, blobType);
            var subId = message.SubmissionId;

            log.LogInformation($"blobject type: {blobject.GetType()}");

            switch (blobject)
            {
                case List<DiagnosisModel> model:
                    if (!model.Any()) break;

                    log.LogInformation($"blobject typematch: {model.GetType()}");
                    switch (message.Operation)
                    {
                        case Operation.Submit:

                            //Transform to a DTO and try to write it
                            var diagnosisDtos = _mapper.Map<IEnumerable<DiagnosisDto>>(model);

                            foreach (var dto in diagnosisDtos)
                            {
                                dto.OrganisationId = biobankId;
                            }

                            try
                            {
                                await _diagnoses.ProcessDiagnoses(diagnosisDtos);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _errors.Add(
                                    subId,
                                    message.Operation,
                                    "Diagnosis",
                                    e.ValidationResults,
                                    message.BiobankId);
                            }

                            await _submissions.ProcessRecords(subId, diagnosisDtos.Count());

                            break;

                        case Operation.Delete:
                            //Transform to a DTO and try to delete it
                            var diagnosisDeletes = _mapper.Map<IEnumerable<DiagnosisDto>>(model);

                            //additional bits automapper won't do
                            foreach (var dto in diagnosisDeletes)
                            {
                                dto.OrganisationId = biobankId;
                            }

                            try
                            {
                                await _diagnoses.DeleteDiagnosesIfExists(diagnosisDeletes);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _errors.Add(
                                    subId,
                                    message.Operation,
                                    "Diagnosis",
                                    e.ValidationResults,
                                    message.BiobankId);
                            }

                            await _submissions.ProcessRecords(subId, diagnosisDeletes.Count());

                            break;
                    }
                    break;

                case List<SampleModel> model:
                    if (!model.Any()) break;

                    log.LogInformation($"blobject typematch: {model.GetType()}");
                    switch (message.Operation)
                    {
                        case Operation.Submit:
                            //Transform to a DTO and try to write it
                            var sampleDtos = _mapper.Map<IEnumerable<SampleDto>>(model);

                            foreach (var dto in sampleDtos)
                            {
                                dto.OrganisationId = biobankId;
                            }

                            try
                            {
                                await _samples.ProcessSamples(sampleDtos);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _errors.Add(
                                    subId,
                                    message.Operation,
                                    "Sample",
                                    e.ValidationResults,
                                    message.BiobankId);
                            }

                            await _submissions.ProcessRecords(subId, sampleDtos.Count());

                            break;

                        case Operation.Delete:
                            //Transform to a DTO and try to delete it
                            var sampleDeletes = _mapper.Map<IEnumerable<SampleDto>>(model);

                            //additional bits automapper won't do
                            foreach (var dto in sampleDeletes)
                            {
                                dto.OrganisationId = biobankId;
                            }

                            try
                            {
                                await _samples.DeleteSamplesIfExists(sampleDeletes);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _errors.Add(
                                    subId,
                                    message.Operation,
                                    "Sample",
                                    e.ValidationResults,
                                    message.BiobankId);
                            }

                            await _submissions.ProcessRecords(subId, sampleDeletes.Count());

                            break;
                    }
                    break;

                case List<TreatmentModel> model:
                    if (!model.Any()) break;

                    log.LogInformation($"blobject typematch: {model.GetType()}");
                    switch (message.Operation)
                    {
                        case Operation.Submit:
                            //Transform to a DTO and try to write it
                            var treatmentDtos = _mapper.Map<IEnumerable<TreatmentDto>>(model);

                            foreach (var dto in treatmentDtos)
                            {
                                dto.OrganisationId = biobankId;
                            }

                            try
                            {
                                await _treatments.ProcessTreatments(treatmentDtos);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _errors.Add(
                                    subId,
                                    message.Operation,
                                    "Treatment",
                                    e.ValidationResults,
                                    message.BiobankId);
                            }

                            await _submissions.ProcessRecords(subId, treatmentDtos.Count());

                            break;

                        case Operation.Delete:
                            //Transform to a DTO and try to delete it
                            var treatmentDeletes = _mapper.Map<IEnumerable<TreatmentDto>>(model);

                            //additional bits automapper won't do
                            foreach (var dto in treatmentDeletes)
                            {
                                dto.OrganisationId = biobankId;
                            }

                            try
                            {
                                await _treatments.DeleteTreatmentsIfExists(treatmentDeletes);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _errors.Add(
                                    subId,
                                    message.Operation,
                                    "Treatment",
                                    e.ValidationResults,
                                    message.BiobankId);
                            }

                            await _submissions.ProcessRecords(subId, treatmentDeletes.Count());

                            break;
                    }
                    break;
            }

            await _blobWriter.DeleteAsync(storageContainer, message.BlobId);
            await _queueWriter.DeleteAsync("operations", id, popReceipt);
        }
    }
}

