using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Entities.Data;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;


namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/DiseaseStatus")]
    public class DiseaseStatusController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public DiseaseStatusController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
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

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListDiagnosesAsync()).Where(x => x.DiagnosisId == id).First();

            if (await _biobankReadService.IsDiagnosisInUse(id))
            {
                ModelState.AddModelError("Description", $"The disease status \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteDiagnosisAsync(new Diagnosis
            {
                DiagnosisId = model.DiagnosisId,
                Description = model.Description
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, DiagnosisModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidDiagnosisDescriptionAsync(id, model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use by another disease status. Disease status descriptions must be unique.");
            }

            if (await _biobankReadService.IsDiagnosisInUse(id))
            {
                ModelState.AddModelError("Description", "This disease status is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateDiagnosisAsync(new Diagnosis
            {
                DiagnosisId = id,
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
        [Route("")]
        public async Task<IHttpActionResult> Post(DiagnosisModel model)
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