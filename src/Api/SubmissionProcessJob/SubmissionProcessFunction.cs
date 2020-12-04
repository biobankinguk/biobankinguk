using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Common.Exceptions;
using Biobanks.Common.Models;
using Biobanks.Common.Types;
using Biobanks.SubmissionProcessJob.Dtos;
using Biobanks.SubmissionProcessJob.Services.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Biobanks.SubmissionProcessJob
{
    public class SubmissionProcessFunction
    {
        // Cloud Services
        private readonly IBlobReadService _blobReadService;
        private readonly IBlobWriteService _blobWriteService;
        private readonly IQueueWriteService _queueWriteService;

        // Automapper
        private readonly IMapper _mapper;

        // Submission Status
        private readonly ISubmissionStatusService _submissionStatusService;

        // Entity Validation and Write Services
        private readonly IDiagnosisWriteService _diagnosisWriteService;
        private readonly ISampleWriteService _sampleWriteService;
        private readonly ITreatmentWriteService _treatmentWriteService;

        public SubmissionProcessFunction(
            IBlobReadService blobReadService,
            IBlobWriteService blobWriteService,
            IQueueWriteService queueWriteService,
            ISubmissionStatusService submissionStatusService,
            IDiagnosisWriteService diagnosisWriteService,
            ISampleWriteService sampleWriteService,
            ITreatmentWriteService treatmentWriteService,
            IMapper mapper)
        {
            _blobReadService = blobReadService;
            _blobWriteService = blobWriteService;
            _queueWriteService = queueWriteService;
            _submissionStatusService = submissionStatusService;
            _diagnosisWriteService = diagnosisWriteService;
            _sampleWriteService = sampleWriteService;
            _treatmentWriteService = treatmentWriteService;
            _mapper = mapper;
        }

        [FunctionName("SubmissionProcessFunction")]
        public async Task Run([QueueTrigger("operations")] CloudQueueMessage incomingMessage, ILogger log)
        {
            const string storageContainer = "submission-payload";
            var message = JsonConvert.DeserializeObject<OperationsQueueItem>(incomingMessage.AsString);
            var blobContents = await _blobReadService.GetObjectFromJsonAsync(storageContainer, message.BlobId);
            var biobankId = message.BiobankId;

            // Get The Type From Stored String
            var blobType = Type.GetType(message.BlobType);

            log.LogInformation($"BlobType: {message.BlobType}");
            log.LogInformation(blobContents);

            var blobject = JsonConvert.DeserializeObject(blobContents, blobType);
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
                                await _diagnosisWriteService.ProcessDiagnoses(diagnosisDtos);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _submissionStatusService.AddErrors(
                                    subId,
                                    message.Operation,
                                    "Diagnosis",
                                    e.ValidationResults,
                                    message.BiobankId);
                            }

                            await _submissionStatusService.ProcessRecords(subId, diagnosisDtos.Count());

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
                                await _diagnosisWriteService.DeleteDiagnosesIfExists(diagnosisDeletes);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _submissionStatusService.AddErrors(
                                    subId,
                                    message.Operation,
                                    "Diagnosis",
                                    e.ValidationResults,
                                    message.BiobankId);
                            }

                            await _submissionStatusService.ProcessRecords(subId, diagnosisDeletes.Count());

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
                                await _sampleWriteService.ProcessSamples(sampleDtos);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _submissionStatusService.AddErrors(
                                    subId,
                                    message.Operation,
                                    "Sample",
                                    e.ValidationResults,
                                    message.BiobankId);
                            }

                            await _submissionStatusService.ProcessRecords(subId, sampleDtos.Count());

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
                                await _sampleWriteService.DeleteSamplesIfExists(sampleDeletes);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _submissionStatusService.AddErrors(
                                    subId,
                                    message.Operation,
                                    "Sample",
                                    e.ValidationResults,
                                    message.BiobankId);
                            }

                            await _submissionStatusService.ProcessRecords(subId, sampleDeletes.Count());

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
                                await _treatmentWriteService.ProcessTreatments(treatmentDtos);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _submissionStatusService.AddErrors(
                                    subId,
                                    message.Operation,
                                    "Treatment",
                                    e.ValidationResults,
                                    message.BiobankId);
                            }

                            await _submissionStatusService.ProcessRecords(subId, treatmentDtos.Count());

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
                                await _treatmentWriteService.DeleteTreatmentsIfExists(treatmentDeletes);
                            }
                            catch (AggregateBiobanksValidationException e)
                            {
                                await _submissionStatusService.AddErrors(
                                    subId,
                                    message.Operation,
                                    "Treatment",
                                    e.ValidationResults,
                                    message.BiobankId);
                            }

                            await _submissionStatusService.ProcessRecords(subId, treatmentDeletes.Count());

                            break;
                    }
                    break;
            }

            await _blobWriteService.DeleteAsync(storageContainer, message.BlobId);
            await _queueWriteService.DeleteAsync("operations", incomingMessage);
        }
    }
}

