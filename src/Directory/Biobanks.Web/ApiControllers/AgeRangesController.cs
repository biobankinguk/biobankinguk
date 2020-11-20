using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Directory.Entity.Data;
using Biobanks.Web.Utilities;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using System.Web.Http.ModelBinding;

namespace Biobanks.Web.ApiControllers
{
    public class AgeRangesController : ApiController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AgeRangesController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        public async Task<IList> AgeRanges()
        {
            var models = (await _biobankReadService.ListAgeRangesAsync())
                .Select(x =>
                    Task.Run(async () => new AgeRangeModel()
                    {
                        Id = x.AgeRangeId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetAgeRangeUsageCount(x.AgeRangeId)
                    })
                    .Result
                )
                .ToList();

            return models;
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddAgeRangeAjax(AgeRangeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAgeRangeAsync(model.Description))
            {
                ModelState.AddModelError("AgeRange", "That description is already in use. Age ranges must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // Add new Age Range
            var range = new AgeRange
            {
                AgeRangeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddAgeRangeAsync(range);
            await _biobankWriteService.UpdateAgeRangeAsync(range, true); // Ensure sortOrder is correct

            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddAgeRangeSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditAgeRangeAjax(AgeRangeModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidAgeRangeAsync(model.Description))
            {
                ModelState.AddModelError("AgeRange", "That description is already in use. Age ranges must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = model.SampleSetsCount > 0;

            // Update Preservation Type
            await _biobankWriteService.UpdateAgeRangeAsync(new AgeRange
            {
                AgeRangeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditAgeRangeSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteAgeRange(AgeRangeModel model)
        {
            if (await _biobankReadService.IsAgeRangeInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The age range \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteAgeRangeAsync(new AgeRange
            {
                AgeRangeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            return Json(new
            {
                msg = $"The age range  \"{model.Description}\" was deleted successfully.",
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