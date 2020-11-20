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
    public class AccessConditionsController : ApiController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AccessConditionsController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService  = biobankWriteService;
        }

        // GET api/{Controller}/{Action}

        [HttpGet]
        public async Task<IList> AccessConditions()
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
        public async Task<IHttpActionResult> AddAccessConditionAjax(AccessConditionModel model)
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
                redirect = $"AddAccessConditionSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditAccessConditionAjax(AccessConditionModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidAccessConditionDescriptionAsync(model.Id, model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use by another access condition. Access condition descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsAccessConditionInUse(model.Id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This access condition is currently in use and cannot be edited." }
                });
            }

            var access = new AccessCondition
            {
                AccessConditionId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.UpdateAccessConditionAsync(access);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditAccessConditionSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteAccessCondition(AccessConditionModel model)
        {
            if (await _biobankReadService.IsAccessConditionInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The access condition \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteAccessConditionAsync(new AccessCondition
            {
                AccessConditionId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The access condition \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }

        private IHttpActionResult JsonModelInvalidResponse(ModelStateDictionary state)
        {
            return Json(new
            {
                success = false,
                errors = state.Values
                    .Where(x => x.Errors.Count > 0)
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }
    }
}