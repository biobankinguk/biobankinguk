using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Common.Auth;
using Biobanks.Common.Models;
using Biobanks.Common.Types;
using Biobanks.SubmissionApi.EqualityComparers;
using Biobanks.SubmissionApi.Models;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.SubmissionApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Controller for handling submissions of data for a biobank
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class SubmitController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ISubmissionService _submissionService;
        private readonly IBlobWriteService _blobWriteService;
        private readonly IQueueWriteService _queueWriteService;

        /// <inheritdoc />
        public SubmitController(IConfiguration config,
            IMapper mapper,
            ISubmissionService submissionService,
            IBlobWriteService blobWriteService,
            IQueueWriteService queueWriteService)
        {
            _config = config;
            _mapper = mapper;
            _submissionService = submissionService;
            _blobWriteService = blobWriteService;
            _queueWriteService = queueWriteService;
        }

        /// <summary>
        /// Inserts or updates a sample.
        /// </summary>
        /// <param name="model">The sample model to be inserted to or updated in staging.</param>
        /// <param name="biobankId">The ID of the biobank to operate on.</param>
        /// <returns>The created content.</returns>
        [HttpPost("{biobankId}")]
        [SwaggerResponse(202, Type = typeof(SubmissionSummaryModel))]
        [SwaggerResponse(400, "Request body expected.")]
        [SwaggerResponse(400, "Invalid request body provided.")]
        [SwaggerResponse(403, "Access to post to the requested biobank denied.")]
        [SwaggerResponse(409, "Newer record exists.")]
        public async Task<IActionResult> Post(int biobankId, [FromBody] SubmissionModel model)
        {
            if (!User.HasClaim(CustomClaimTypes.BiobankId,
                biobankId.ToString()))
                return Forbid();

            // validate the model
            if (model is null) return BadRequest("Request body expected.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // initialise any null lists in the model
            if (model.Diagnoses is null) model.Diagnoses = new List<DiagnosisOperationModel>();
            if (model.Treatments is null) model.Treatments = new List<TreatmentOperationModel>();
            if (model.Samples is null) model.Samples = new List<SampleOperationModel>();

            var totalRecords = model.Diagnoses.Count + model.Samples.Count + model.Treatments.Count;

            if (totalRecords <= 0) return BadRequest("At least one record must be included in a submission.");

            if (SectionsWithDuplicates(model, out var sections))
                return BadRequest(
                    $"This submission contains multiple entries with matching identifiers in the following sections: {string.Join('.', sections)}");

            var maxEntitiesPerSubmission = _config.GetValue<int>("Limits:EntitiesPerSubmission");
            if (model.Diagnoses.Count + model.Samples.Count + model.Treatments.Count > maxEntitiesPerSubmission)
            {
                return BadRequest($"This submission contains more than the maximum of {maxEntitiesPerSubmission} records allowed.");
            }

            var diagnosesUpdates = new List<DiagnosisModel>();
            var samplesUpdates = new List<SampleModel>();
            var treatmentsUpdates = new List<TreatmentModel>();
            var diagnosesDeletes = new List<DiagnosisModel>();
            var samplesDeletes = new List<SampleModel>();
            var treatmentsDeletes = new List<TreatmentModel>();

            var submission = await _submissionService.CreateSubmission(totalRecords, biobankId);

            // Validate diagnosis mandatory fields and add to submission
            foreach (var diagnosis in model.Diagnoses)
            {
                switch (diagnosis.Op)
                {
                    case Operation.Submit:
                        var diagnosisModel = _mapper.Map<DiagnosisIdModel, DiagnosisModel>(diagnosis.Diagnosis,
                            opts =>
                                opts.AfterMap((src, dest) =>
                                {
                                    dest.SubmissionTimestamp = submission.SubmissionTimestamp;
                                }));

                        if (string.IsNullOrEmpty(diagnosisModel.DiagnosisCode))
                            return await CancelSubmissionAndReturnBadRequest(diagnosisModel, submission.Id, "Invalid DiagnosisCode value.");
                        else if (string.IsNullOrEmpty(diagnosisModel.DiagnosisCodeOntology))
                            return await CancelSubmissionAndReturnBadRequest(diagnosisModel, submission.Id, "Invalid DiagnosisCodeOntology value.");
                        else if (string.IsNullOrEmpty(diagnosisModel.DiagnosisCodeOntologyVersion))
                            return await CancelSubmissionAndReturnBadRequest(diagnosisModel, submission.Id, "Invalid DiagnosisCodeOntologyVersion value.");
                        else if (diagnosisModel.DateDiagnosed == null)
                            return await CancelSubmissionAndReturnBadRequest(diagnosisModel, submission.Id, "Invalid DateDiagnosed value.");
                        else
                            diagnosesUpdates.Add(diagnosisModel);
                        break;

                    case Operation.Delete:
                        diagnosesDeletes.Add(_mapper.Map<DiagnosisIdModel, DiagnosisModel>(diagnosis.Diagnosis,
                            opts =>
                                opts.AfterMap((src, dest) =>
                                {
                                    dest.SubmissionTimestamp = submission.SubmissionTimestamp;
                                })));
                        break;

                    default:
                        return await CancelSubmissionAndReturnBadRequest(_mapper.Map<DiagnosisIdModel, DiagnosisModel>(diagnosis.Diagnosis), submission.Id, "Invalid operation specified.");

                }
            }

            // Validate sample mandatory fields and add to submission
            foreach (var sample in model.Samples)
            {
                switch (sample.Op)
                {
                    case Operation.Submit:
                        var sampleModel = _mapper.Map<SampleSubmissionModel, SampleModel>(sample.Sample, opts =>
                            opts.AfterMap((src, dest) =>
                            {
                                dest.SubmissionTimestamp = submission.SubmissionTimestamp;
                            }));

                        if (string.IsNullOrEmpty(sampleModel.MaterialType))
                            return await CancelSubmissionAndReturnBadRequest(sampleModel, submission.Id, "Invalid MaterialType value.");
                        else if (string.IsNullOrEmpty(sampleModel.StorageTemperature))
                            return await CancelSubmissionAndReturnBadRequest(sampleModel, submission.Id, "Invalid StorageTemperature value.");
                        else if (sampleModel.AgeAtDonation == null && sampleModel.YearOfBirth == null)
                            return await CancelSubmissionAndReturnBadRequest(sampleModel, submission.Id, "At least one of AgeAtDonation or YearOfBirth must be provided.");
                        else if (sampleModel.DateCreated == null)
                            return await CancelSubmissionAndReturnBadRequest(sampleModel, submission.Id, "Invalid DateCreated value.");

                        else
                            samplesUpdates.Add(sampleModel);
                        break;

                    case Operation.Delete:
                        samplesDeletes.Add(_mapper.Map<SampleSubmissionModel, SampleModel>(sample.Sample, opts =>
                            opts.AfterMap((src, dest) =>
                            {
                                dest.SubmissionTimestamp = submission.SubmissionTimestamp;
                            })));
                        break;

                    default:
                        return await CancelSubmissionAndReturnBadRequest(_mapper.Map<SampleSubmissionModel, SampleModel>(sample.Sample), submission.Id, "Invalid operation specified.");
                }

            }

            // Validate treatment mandatory fields and add to submission
            foreach (var treatment in model.Treatments)
            {
                switch (treatment.Op)
                {
                    case Operation.Submit:
                        var treatmentModel = _mapper.Map<TreatmentSubmissionModel, TreatmentModel>(treatment.Treatment,
                            opts =>
                                opts.AfterMap((src, dest) =>
                                {
                                    dest.SubmissionTimestamp = submission.SubmissionTimestamp;
                                }));

                        if (string.IsNullOrEmpty(treatmentModel.TreatmentLocation))
                            return await CancelSubmissionAndReturnBadRequest(treatmentModel, submission.Id, "Invalid TreatmentLocation value.");
                        else if (string.IsNullOrEmpty(treatmentModel.TreatmentCodeOntology))
                            return await CancelSubmissionAndReturnBadRequest(treatmentModel, submission.Id, "Invalid TreatmentCodeOntology value.");
                        else if (string.IsNullOrEmpty(treatmentModel.TreatmentCodeOntologyVersion))
                        return await CancelSubmissionAndReturnBadRequest(treatmentModel, submission.Id, "Invalid TreatmentCodeOntologyVersion value.");
                        else if (treatmentModel.DateTreated == null)
                            return await CancelSubmissionAndReturnBadRequest(treatmentModel, submission.Id, "Invalid DateTreated value.");
                        else
                            treatmentsUpdates.Add(treatmentModel);
                        break;

                    case Operation.Delete:
                        treatmentsDeletes.Add(_mapper.Map<TreatmentSubmissionModel, TreatmentModel>(treatment.Treatment,
                            opts =>
                                opts.AfterMap((src, dest) =>
                                {
                                    dest.SubmissionTimestamp = submission.SubmissionTimestamp;
                                })));
                        break;

                    default:
                        return await CancelSubmissionAndReturnBadRequest(_mapper.Map<TreatmentIdModel, TreatmentModel>(treatment.Treatment), submission.Id, "Invalid operation specified.");
                }

            }
            
            // Send the diagnosis insert/updates up to queue
            var diagnosesUpdatesBlobId =
                await _blobWriteService.StoreObjectAsJsonAsync("submission-payload", diagnosesUpdates);

            await _queueWriteService.PushAsync("operations",
                JsonConvert.SerializeObject(
                    new OperationsQueueItem
                    {
                        SubmissionId = submission.Id,
                        Operation = Operation.Submit,
                        BlobId = diagnosesUpdatesBlobId,
                        BlobType = diagnosesUpdates.GetType().FullName,
                        BiobankId = biobankId
                    }
                )
            );

            // Send the diagnosis deletes up to queue
            var diagnosesDeletesBlobId =
                await _blobWriteService.StoreObjectAsJsonAsync("submission-payload", diagnosesDeletes);

            await _queueWriteService.PushAsync("operations",
                JsonConvert.SerializeObject(
                    new OperationsQueueItem
                    {
                        SubmissionId = submission.Id,
                        Operation = Operation.Delete,
                        BlobId = diagnosesDeletesBlobId,
                        BlobType = diagnosesDeletes.GetType().FullName,
                        BiobankId = biobankId
                    }
                )
            );

            // Send the sample insert/updates up to queue
            var samplesUpdatesBlobId =
                await _blobWriteService.StoreObjectAsJsonAsync("submission-payload", samplesUpdates);

            await _queueWriteService.PushAsync("operations",
                JsonConvert.SerializeObject(
                    new OperationsQueueItem
                    {
                        SubmissionId = submission.Id,
                        Operation = Operation.Submit,
                        BlobId = samplesUpdatesBlobId,
                        BlobType = samplesUpdates.GetType().FullName,
                        BiobankId = biobankId
                    }
                )
            );

            // Send the sample deletes up to queue
            var samplesDeletesBlobId =
                await _blobWriteService.StoreObjectAsJsonAsync("submission-payload", samplesDeletes);

            await _queueWriteService.PushAsync("operations",
                JsonConvert.SerializeObject(
                    new OperationsQueueItem
                    {
                        SubmissionId = submission.Id,
                        Operation = Operation.Delete,
                        BlobId = samplesDeletesBlobId,
                        BlobType = samplesDeletes.GetType().FullName,
                        BiobankId = biobankId
                    }
                )
            );

            // Send the treatment insert/updates up to queue
            var treatmentsUpdatesBlobId =
                await _blobWriteService.StoreObjectAsJsonAsync("submission-payload", treatmentsUpdates);

            await _queueWriteService.PushAsync("operations",
                JsonConvert.SerializeObject(
                    new OperationsQueueItem
                    {
                        SubmissionId = submission.Id,
                        Operation = Operation.Submit,
                        BlobId = treatmentsUpdatesBlobId,
                        BlobType = treatmentsUpdates.GetType().FullName,
                        BiobankId = biobankId
                    }
                )
            );

            // Send the treatment deletes up to queue
            var treatmentsDeletesBlobId =
                await _blobWriteService.StoreObjectAsJsonAsync("submission-payload", treatmentsDeletes);

            await _queueWriteService.PushAsync("operations",
                JsonConvert.SerializeObject(
                    new OperationsQueueItem
                    {
                        SubmissionId = submission.Id,
                        Operation = Operation.Delete,
                        BlobId = treatmentsDeletesBlobId,
                        BlobType = treatmentsDeletes.GetType().FullName,
                        BiobankId = biobankId
                    }
                )
            );

            // return the status object
            return Accepted(_mapper.Map<SubmissionSummaryModel>(submission));
        }

        private async Task<BadRequestObjectResult> CancelSubmissionAndReturnBadRequest(object badEntity, int submissionId, string errorText)
        {
            await _submissionService.DeleteSubmission(submissionId);
            return BadRequest($"{IdPropertiesPrefix(badEntity)}{errorText}");
        }

        private static bool SectionsWithDuplicates(SubmissionModel submission, out List<string> sections)
        {
            sections = new List<string>();

            IEqualityComparer<T> GetComparer<T>(ICollection<T> models)
            {
                switch (models)
                {
                    case ICollection<DiagnosisOperationModel> d:
                        return (IEqualityComparer<T>)new DiagnosisOperationModelEqualityComparer();
                    case ICollection<TreatmentOperationModel> t:
                        return (IEqualityComparer<T>)new TreatmentOperationModelEqualityComparer();
                    case ICollection<SampleOperationModel> s:
                        return (IEqualityComparer<T>)new SampleOperationModelEqualityComparer();
                    default: throw new InvalidOperationException();
                }
            }

            bool NoDuplicates<T>(ICollection<T> models)
                where T : BaseOperationModel
                => models.Count == models.Distinct(GetComparer(models)).Count();

            if (!NoDuplicates(submission.Diagnoses)) sections.Add("Diagnosis");
            if (!NoDuplicates(submission.Treatments)) sections.Add("Treatment");
            if (!NoDuplicates(submission.Samples)) sections.Add("Sample");

            return sections.Any();
        }

        private static string IdPropertiesPrefix(object entity)
        {
            switch (entity)
            {
                // TODO make ontology/ontologyversion identifying properties of each entity then use reflection to get prop names from xIdModels, excluding submissionTimestamp and organisationId
                case DiagnosisModel model:
                    return $"{nameof(model.IndividualReferenceId)}: {model.IndividualReferenceId}, {nameof(model.DateDiagnosed)}: {model.DateDiagnosed}, {nameof(model.DiagnosisCode)}: {model.DiagnosisCode} - ";
                case SampleModel model:
                    return $"{nameof(model.IndividualReferenceId)}: {model.IndividualReferenceId}, {nameof(model.Barcode)}: {model.Barcode}, {nameof(model.CollectionName)}: {model.CollectionName} - ";
                case TreatmentModel model:
                    return $"{nameof(model.IndividualReferenceId)}: {model.IndividualReferenceId}, {nameof(model.DateTreated)}: {model.DateTreated}, {nameof(model.TreatmentCode)}: {model.TreatmentCode} - ";
                default:
                    throw new ArgumentException($"{nameof(entity)} is not a valid identity model.", nameof(entity));
            }
        }
    }
}
