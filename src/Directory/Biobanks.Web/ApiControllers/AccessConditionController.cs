using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/AccessCondition")]
    public class AccessConditionController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AccessConditionController(IBiobankReadService biobankReadService,
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
            var models = (await _biobankReadService.ListAccessConditionsAsync())
                .Select(x =>
                    Task.Run(async () => new ReadAccessConditionsModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        AccessConditionCount = await _biobankReadService.GetAccessConditionsCount(x.Id),
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
                Id = model.Id,
                Value = model.Description,
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
                Id = id,
                Value = model.Description,
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
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListAccessConditionsAsync()).Where(x => x.Id == id).First();

            // If in use, prevent update
            if (await _biobankReadService.IsAccessConditionInUse(id))
            {
                ModelState.AddModelError("Description", $"The access condition \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteAccessConditionAsync(new AccessCondition
            {
                Id = model.Id
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, AccessConditionModel model)
        {
            var access = new AccessCondition
            {
                Id = id,
                Value = model.Description,
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