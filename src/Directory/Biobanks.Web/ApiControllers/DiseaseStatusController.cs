using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Entities.Shared.ReferenceData;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/DiseaseStatus")]
    public class DiseaseStatusController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public DiseaseStatusController(IBiobankReadService biobankReadService, IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            return (await _biobankReadService.ListSnomedTermsAsync()).Select(x =>

                Task.Run(async () => new ReadSnomedTermModel
                {
                    SnomedTermId = x.Id,
                    Description = x.Description,
                    CollectionCapabilityCount = await _biobankReadService.GetSnomedTermCollectionCapabilityCount(x.Id),
                    OtherTerms = x.OtherTerms
                })
                .Result
            )
            .ToList();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            var model = (await _biobankReadService.ListSnomedTermsAsync()).Where(x => x.Id == id).First();

            if (await _biobankReadService.IsSnomedTermInUse(id))
            {
                ModelState.AddModelError("Description", $"The disease status \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteSnomedTermAsync(new SnomedTerm
            {
                Id = model.Id,
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
        public async Task<IHttpActionResult> Put(string id, SnomedTermModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidSnomedTermDescriptionAsync(id, model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use by another disease status. Disease status descriptions must be unique.");
            }

            if (await _biobankReadService.IsSnomedTermInUse(id))
            {
                ModelState.AddModelError("Description", "This disease status is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateSnomedTermAsync(new SnomedTerm
            {
                Id = model.SnomedTermId,
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
        public async Task<IHttpActionResult> Post(SnomedTermModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidSnomedTermDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Disease status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddSnomedTermAsync(new SnomedTerm
            {
                Id = model.SnomedTermId,
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
    }
}