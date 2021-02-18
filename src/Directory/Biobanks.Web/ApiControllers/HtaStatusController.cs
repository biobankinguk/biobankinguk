using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.Shared;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/HtaStatus")]
    public class HtaStatusController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public HtaStatusController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _biobankReadService.ListHtaStatusesAsync())
                        .Select(x =>

                    Task.Run(async () => new Models.ADAC.ReadHtaStatusModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        CollectionCount = await _biobankReadService.GetHtaStatusCollectionCount(x.Id),
                        SortOrder = x.SortOrder
                    }).Result)

                        .ToList();

         return models;

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListHtaStatusesAsync()).Where(x => x.Id == id).First();

            if (await _biobankReadService.IsHtaStatusInUse(id))
            {
                ModelState.AddModelError("HtaStatus", $"The hta status \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteHtaStatusAsync(new HtaStatus
            {
                Id = model.Id,
                Value = model.Value
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, HtaStatusModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidHtaStatusDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("HtaStatus", "That hta status already exists!");
            }

            if (await _biobankReadService.IsHtaStatusInUse(id))
            {
                ModelState.AddModelError("HtaStatus", "This hta status is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateHtaStatusAsync(new HtaStatus
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
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
        public async Task<IHttpActionResult> Post(HtaStatusModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidHtaStatusDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Hta status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddHtaStatusAsync(new HtaStatus
            {
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, HtaStatusModel model)
        {
            await _biobankWriteService.UpdateHtaStatusAsync(new HtaStatus
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            },
            true);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });

        }

    }
}