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
    [RoutePrefix("api/AccessConditions")]
    public class AccessConditionsController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AccessConditionsController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _biobankReadService.ListAccessConditionsAsync())
                .Select(x =>
                    Task.Run(async () => new ReadAccessConditionsModel
                    {
                        Id = x.AccessConditionId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        AccessConditionCount = await _biobankReadService.GetAccessConditionsCount(x.AccessConditionId),
                    }
                    )
                    .Result
                )
                .ToList();

            return models;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(AccessConditionModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidAccessConditionDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Access condition descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var access = new AccessCondition
            {
                AccessConditionId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddAccessConditionAsync(access);
            await _biobankWriteService.UpdateAccessConditionAsync(access, true);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, AccessConditionModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidAccessConditionDescriptionAsync(id, model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use by another access condition. Access condition descriptions must be unique.");
            }

            // If in use, prevent update
            if (await _biobankReadService.IsAccessConditionInUse(id))
            {
                ModelState.AddModelError("Description", $"The access condition \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var access = new AccessCondition
            {
                AccessConditionId = id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.UpdateAccessConditionAsync(access);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListAccessConditionsAsync()).Where(x => x.AccessConditionId == id).First();

            if (await _biobankReadService.IsAccessConditionInUse(model.AccessConditionId))
            {
                return Json(new
                {
                    msg = $"The access condition \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteAccessConditionAsync(new AccessCondition
            {
                AccessConditionId = model.AccessConditionId
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The access condition \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }

        [HttpPut]
        [Route("Sort/{id}")]
        public async Task<IHttpActionResult> Sort(int id, AccessConditionModel model)
        {
            var access = new AccessCondition
            {
                AccessConditionId = id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.UpdateAccessConditionAsync(access, true);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });

        }

    }
}