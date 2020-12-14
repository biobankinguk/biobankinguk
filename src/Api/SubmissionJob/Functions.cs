using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Common.Exceptions;
using Biobanks.Common.Models;
using Biobanks.Common.Types;
using Biobanks.SubmissionJob.Dtos;
using Biobanks.SubmissionJob.Services.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Biobanks.SubmissionJob
{
    public class Functions
    {
        // cloud services
        private readonly IBlobReadService _blobReadService;
        private readonly IBlobWriteService _blobWriteService;
        private readonly IQueueWriteService _queueWriteService;

        // automapper
        private readonly IMapper _mapper;

        // submission status
        private readonly ISubmissionStatusService _submissionStatusService;

        // entity validation and write services
        private readonly IDiagnosisWriteService _diagnosisWriteService;
        private readonly ISampleWriteService _sampleWriteService;
        private readonly ITreatmentWriteService _treatmentWriteService;

        public Functions(IBlobReadService blobReadService,
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

        public async Task ProcessQueueMessageAsync([QueueTrigger("operations")] CloudQueueMessage incomingMessage, TextWriter log)
        {
            const string storageContainer = "submission-payload";
            var message = JsonConvert.DeserializeObject<OperationsQueueItem>(incomingMessage.AsString);
            var blobContents = await _blobReadService.GetObjectFromJsonAsync(storageContainer, message.BlobId);
            var biobankId = message.BiobankId;

            var blobType = Type.GetType(message.BlobType); //get the type back from the stored string

            log.WriteLine($"BlobType: {message.BlobType}");

            log.WriteLine(blobContents);

            var blobject = JsonConvert.DeserializeObject(blobContents, blobType);
            log.WriteLine($"blobject type: {blobject.GetType()}");

            var subId = message.SubmissionId;

            switch (blobject)
            {
                case List<DiagnosisModel> model:
                    if (!model.Any()) break;

                    log.WriteLine($"blobject typematch: {model.GetType()}");
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

                    log.WriteLine($"blobject typematch: {model.GetType()}");
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

                    log.WriteLine($"blobject typematch: {model.GetType()}");
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
