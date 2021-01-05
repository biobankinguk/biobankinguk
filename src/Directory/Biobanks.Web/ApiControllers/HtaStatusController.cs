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
using Directory.Data.Constants;

namespace Biobanks.Web.ApiControllers
{
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
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _biobankReadService.ListHtaStatusesAsync())
                        .Select(x =>

                    Task.Run(async () => new ReadHtaStatusModel
                    {
                        Id = x.HtaStatusId,
                        Description = x.Description,
                        CollectionCount = await _biobankReadService.GetHtaStatusCollectionCount(x.HtaStatusId),
                        SortOrder = x.SortOrder
                    }).Result)

                        .ToList();

         return models;

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListHtaStatusesAsync()).Where(x => x.HtaStatusId == id).First();

            if (await _biobankReadService.IsHtaStatusInUse(id))
            {
                ModelState.AddModelError("HtaStatus", $"The hta status \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteHtaStatusAsync(new HtaStatus
            {
                HtaStatusId = model.HtaStatusId,
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
        public async Task<IHttpActionResult> Put(int id, Models.Shared.HtaStatusModel model)
        {
            // Validate model
            if (!await _biobankReadService.ValidHtaStatusDescriptionAsync(model.Description))
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

            // Update Preservation Type
            await _biobankWriteService.UpdateHtaStatusAsync(new HtaStatus
            {
                HtaStatusId = id,
                Description = model.Description,
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
        public async Task<IHttpActionResult> Post(Models.Shared.HtaStatusModel model)
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
                Description = model.Description,
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
        public async Task<IHttpActionResult> Move(int id, Models.Shared.HtaStatusModel model)
        {
            await _biobankWriteService.UpdateHtaStatusAsync(new HtaStatus
            {
                HtaStatusId = id,
                Description = model.Description,
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