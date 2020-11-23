using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Directory.Entity.Data;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Utilities;
using Biobanks.Web.Models.ADAC;
using System.Web.Http.Results;
using System.Collections;
using System.Web.Http.ModelBinding;


namespace Biobanks.Web.ApiControllers
{
    public class DiseaseStatusesController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public DiseaseStatusesController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        public async Task<IList> DiseaseStatuses()
        {
            var model = (await _biobankReadService.ListDiagnosesAsync())
                .Select(x =>

                Task.Run(async () => new ReadDiagnosisModel
                {
                    Id = x.DiagnosisId,
                    SnomedIdentifier = x.SnomedIdentifier,
                    Description = x.Description,
                    CollectionCapabilityCount = await _biobankReadService.GetDiagnosisCollectionCapabilityCount(x.DiagnosisId),
                    OtherTerms = x.OtherTerms
                }).Result).ToList();

                return model;   
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteDiseaseStatus(DiagnosisModel model)
        {
            if (await _biobankReadService.IsDiagnosisInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The disease status \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteDiagnosisAsync(new Diagnosis
            {
                DiagnosisId = model.Id,
                Description = model.Description
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The disease status \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditDiseaseStatusAjax(DiagnosisModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidDiagnosisDescriptionAsync(model.Id, model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use by another disease status. Disease status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsDiagnosisInUse(model.Id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This disease status is currently in use and cannot be edited." }
                });
            }

            await _biobankWriteService.UpdateDiagnosisAsync(new Diagnosis
            {
                DiagnosisId = model.Id,
                SnomedIdentifier = model.SnomedIdentifier,
                Description = model.Description,
                OtherTerms = model.OtherTerms
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddDiseaseStatusAjax(DiagnosisModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidDiagnosisDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Disease status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddDiagnosisAsync(new Diagnosis
            {
                Description = model.Description,
                SnomedIdentifier = model.SnomedIdentifier,
                OtherTerms = model.OtherTerms
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }
    }
}