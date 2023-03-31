using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using AutoMapper;
using Biobanks.Directory.Auth;
using Biobanks.Directory.EqualityComparers;
using Biobanks.Directory.Models.Submissions;
using Biobanks.Directory.Services.Submissions.Contracts;
using Biobanks.Submissions.Models;
using Biobanks.Submissions.Services.Contracts;
using Biobanks.Submissions.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.Directory.Controllers.Submissions
{
    /// <summary>
    /// Controller for handling submissions of data for a biobank
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Submissions")]
    [Authorize(nameof(AuthPolicies.IsTokenAuthenticated))]
    public class SubmitController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ISubmissionService _submissionService;
        private readonly IBlobWriteService _blobWriteService;
        private readonly IBackgroundJobEnqueueingService _backgroundQueueService;

        /// <inheritdoc />
        public SubmitController(IConfiguration config,
            IMapper mapper,
            ISubmissionService submissionService,
            IBlobWriteService blobWriteService,
            IBackgroundJobEnqueueingService backgroundQueueService)
        {
            _config = config;
            _mapper = mapper;
            _submissionService = submissionService;
            _blobWriteService = blobWriteService;
            _backgroundQueueService = backgroundQueueService;
        }

        /// <summary>
        /// Inserts or updates a sample.
        /// </summary>
        /// <param name="biobankId">The ID of the biobank to operate on.</param>
        /// <param name="model">The sample model to be inserted to or updated in staging.</param>
        /// <returns>The created content.</returns>
        [HttpPost("{biobankId}")]
        [SwaggerResponse(202, Type = typeof(SubmissionSummaryModel))]
        [SwaggerResponse(400, "Request body expected.")]
        [SwaggerResponse(400, "Invalid request body provided.")]
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

            // Check Total Records Is Within Range (0, max]
            var totalRecords = model.Diagnoses.Count + model.Samples.Count + model.Treatments.Count;
            var totalMaximum = _config.GetValue<int>("Limits:EntitiesPerSubmission");

            if (totalRecords <= 0)
            {
                return BadRequest("At least one record must be included in a submission.");
            }
            else if (totalRecords > totalMaximum)
            {
                return BadRequest($"This submission contains more than the maximum of {totalMaximum} records allowed.");
            }

            // Check No Section Contains Duplicate Entries
            var duplicates = SectionsWithDuplicates(model);

            if (duplicates.Any())
            {
                return BadRequest(
                    $"This submission contains multiple entries with matching identifiers in the following sections: {string.Join('.', duplicates)}");
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
                        else if (string.IsNullOrEmpty(sampleModel.AgeAtDonation) && sampleModel.YearOfBirth == null)
                            return await CancelSubmissionAndReturnBadRequest(sampleModel, submission.Id, "At least one of AgeAtDonation or YearOfBirth must be provided.");
                        else if (!string.IsNullOrEmpty(sampleModel.AgeAtDonation))
                        {
                            int ageAtDonationInt;
                            if (int.TryParse(sampleModel.AgeAtDonation, out ageAtDonationInt))
                            {
                                if (ageAtDonationInt > 150)
                                {
                                    return await CancelSubmissionAndReturnBadRequest(sampleModel, submission.Id, "Invalid AgeAtDonation value, must not be greater than 150.");
                                }
                                // Check if negative
                                bool isNegative = false;
                                if (ageAtDonationInt < 0)
                                {
                                    isNegative = true;
                                    sampleModel.AgeAtDonation = sampleModel.AgeAtDonation.Replace("-", "");
                                }
                                // Convert to ISO Duration
                                sampleModel.AgeAtDonation = "P" + sampleModel.AgeAtDonation + "Y";
                                if (isNegative)
                                {
                                    sampleModel.AgeAtDonation = "-" + sampleModel.AgeAtDonation;
                                }
                            }
                            else
                            {
                                try
                                {
                                    // Checks if AgeAtDonation is a valid ISO Duration 
                                    XmlConvert.ToTimeSpan(sampleModel.AgeAtDonation);
                                }
                                catch
                                {
                                    return await CancelSubmissionAndReturnBadRequest(sampleModel, submission.Id, "Invalid AgeAtDonation value, must be a valid ISO duration.");
                                }
                            }

                        }
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

                        if (string.IsNullOrEmpty(treatmentModel.TreatmentCodeOntology))
                            return await CancelSubmissionAndReturnBadRequest(treatmentModel, submission.Id, "Invalid TreatmentCodeOntology value.");
                        else if (string.IsNullOrEmpty(treatmentModel.TreatmentCodeOntologyVersion))
                            return await CancelSubmissionAndReturnBadRequest(treatmentModel, submission.Id, "Invalid TreatmentCodeOntologyVersion value.");
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

            // Push Diagnoses Updates/Inserts To Queue
            if (diagnosesUpdates.Any())
            {
                var blobId = await _blobWriteService.StoreObjectAsJsonAsync("submission-payload", diagnosesUpdates);
                var blobType = diagnosesUpdates.GetType().FullName;

                await _backgroundQueueService.Stage(biobankId, submission.Id, blobId, blobType, Operation.Submit);
            }

            // Push Diagnoses Deletes To Queue
            if (diagnosesDeletes.Any())
            {
                var blobId = await _blobWriteService.StoreObjectAsJsonAsync("submission-payload", diagnosesDeletes);
                var blobType = diagnosesDeletes.GetType().FullName;

                await _backgroundQueueService.Stage(biobankId, submission.Id, blobId, blobType, Operation.Delete);
            }

            // Push Samples Updates/Inserts To Queue
            if (samplesUpdates.Any())
            {
                var blobId = await _blobWriteService.StoreObjectAsJsonAsync("submission-payload", samplesUpdates);
                var blobType = samplesUpdates.GetType().FullName;

                await _backgroundQueueService.Stage(biobankId, submission.Id, blobId, blobType, Operation.Submit);
            }

            // Push Samples Deletes To Queue
            if (samplesDeletes.Any())
            {
                var blobId = await _blobWriteService.StoreObjectAsJsonAsync("submission-payload", samplesDeletes);
                var blobType = samplesDeletes.GetType().FullName;

                await _backgroundQueueService.Stage(biobankId, submission.Id, blobId, blobType, Operation.Delete);
            }

            // Push Treatments Updates/Inserts To Queue
            if (treatmentsUpdates.Any())
            {
                var blobId = await _blobWriteService.StoreObjectAsJsonAsync("submission-payload", treatmentsUpdates);
                var blobType = treatmentsUpdates.GetType().FullName;

                await _backgroundQueueService.Stage(biobankId, submission.Id, blobId, blobType, Operation.Submit);
            }

            // Push Treatments Deletes To Queue
            if (treatmentsDeletes.Any())
            {
                // Send the treatment deletes up to queue
                var blobId = await _blobWriteService.StoreObjectAsJsonAsync("submission-payload", treatmentsDeletes);
                var blobType = treatmentsDeletes.GetType().FullName;

                await _backgroundQueueService.Stage(biobankId, submission.Id, blobId, blobType, Operation.Delete);
            }

            // return the status object
            return Accepted(_mapper.Map<SubmissionSummaryModel>(submission));
        }

        private async Task<BadRequestObjectResult> CancelSubmissionAndReturnBadRequest(object badEntity, int submissionId, string errorText)
        {
            await _submissionService.DeleteSubmission(submissionId);
            return BadRequest($"{IdPropertiesPrefix(badEntity)}{errorText}");
        }

        private static IEnumerable<string> SectionsWithDuplicates(SubmissionModel submission)
        {
            var sections = new List<string>();

            if (HasDuplicates(submission.Diagnoses, new DiagnosisOperationModelEqualityComparer()))
            {
                sections.Add("Diagnosis");
            }
            if (HasDuplicates(submission.Treatments, new TreatmentOperationModelEqualityComparer()))
            {
                sections.Add("Treatment");
            }
            if (HasDuplicates(submission.Samples, new SampleOperationModelEqualityComparer()))
            {
                sections.Add("Sample");
            }

            return sections;
        }

        private static bool HasDuplicates<T>(IEnumerable<T> collection, IEqualityComparer<T> comparer) where T : class
        {
            return collection.Distinct(comparer).Count() < collection.Count();
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
