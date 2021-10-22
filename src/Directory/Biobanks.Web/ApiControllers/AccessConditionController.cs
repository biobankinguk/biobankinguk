using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/AccessCondition")]
    public class AccessConditionController : ApiBaseController
    {
        private readonly IReferenceDataService<AccessCondition> _accessConditionService;

        public AccessConditionController(IReferenceDataService<AccessCondition>  accessConditionService)
        {
            _accessConditionService = accessConditionService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _accessConditionService.List())
                .Select(x =>
                    Task.Run(async () => new ReadAccessConditionsModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        AccessConditionCount = await _accessConditionService.GetUsageCount(x.Id),
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
            if (await _accessConditionService.Exists(model.Description))
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

            await _accessConditionService.Add(access);
            await _accessConditionService.Update(access);

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
            var existing = await _accessConditionService.Get(model.Description);

            //If this description is valid, it already exists
            if (existing != null)
            {
                if (existing.Id != id)
                {
                    ModelState.AddModelError("Description", "That description is already in use by another access condition. Access condition descriptions must be unique.");
                }
            }

            // If in use, prevent update
            if (await _accessConditionService.IsInUse(id))
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

            await _accessConditionService.Update(access);

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
            var model = await _accessConditionService.Get(id);

            // If in use, prevent update
            if (await _accessConditionService.IsInUse(id))
            {
                ModelState.AddModelError("Description", $"The access condition \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _accessConditionService.Delete(id);
 
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

            await _accessConditionService.Update(access);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });

        }

    }
}