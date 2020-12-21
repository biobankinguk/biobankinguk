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
        [Route("")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListHtaStatusesAsync()).Where(x => x.HtaStatusId == id).First();

            if (await _biobankReadService.IsHtaStatusInUse(id))
            {
                return Json(new
                {
                    msg = $"The hta status \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteHtaStatusAsync(new HtaStatus
            {
                HtaStatusId = model.HtaStatusId,
                Description = model.Description
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The hta status \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
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

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }
            // If in use, then only re-order the type
            bool inUse = await _biobankReadService.IsHtaStatusInUse(id);

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

        [HttpPut]
        [Route("Sort/{id}")]
        public async Task<IHttpActionResult> Sort(int id, Models.Shared.HtaStatusModel model)
        {

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

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