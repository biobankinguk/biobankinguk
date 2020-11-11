using Biobanks.Web.Models.Shared;
using Biobanks.Web.Utilities;
using Biobanks.Web.Models.ADAC;
using System.Web.Http.Results;
using System.Collections;
using System.Web.Http.ModelBinding;
using System.Web.Http;
using System.Threading.Tasks;
using Directory.Services.Contracts;
using System.Linq;
using Directory.Entity.Data;

namespace Biobanks.Web.ApiControllers
{
    public class AnnualStatisticsController : ApiController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AnnualStatisticsController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }


        // GET: AnnualStatistics
        [HttpGet]
        public async Task<IHttpActionResult> AnnualStatistics()
        {
            var groups = (await _biobankReadService.ListAnnualStatisticGroupsAsync())
                 .Select(x => new AnnualStatisticGroupModel
                 {
                     AnnualStatisticGroupId = x.AnnualStatisticGroupId,
                     Name = x.Name,
                 })
                 .ToList();

            var models = (await _biobankReadService.ListAnnualStatisticsAsync())
                .Select(x =>
                    Task.Run(async () => new AnnualStatisticModel
                    {
                        Id = x.AnnualStatisticId,
                        Name = x.Name,
                        UsageCount = await _biobankReadService.GetAnnualStatisticUsageCount(x.AnnualStatisticId),
                        AnnualStatisticGroupId = x.AnnualStatisticGroupId,
                        AnnualStatisticGroupName = groups.Where(y => y.AnnualStatisticGroupId == x.AnnualStatisticGroupId).FirstOrDefault()?.Name,
                    })
                    .Result
                )
                .ToList();

            return Json(new 
            {
                AnnualStatistics = models,
                AnnualStatisticGroups = groups
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddAnnualStatisticAjax(AnnualStatisticModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAnnualStatisticAsync(model.Name, model.AnnualStatisticGroupId))
            {
                ModelState.AddModelError("AnnualStatistics", "That name is already in use. Annual statistics names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var annualStatistic = new AnnualStatistic
            {
                AnnualStatisticId = model.Id,
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Name = model.Name
            };

            await _biobankWriteService.AddAnnualStatisticAsync(annualStatistic);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
                redirect = $"AddAnnualStatisticSuccess?name={model.Name}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditAnnualStatisticAjax(AnnualStatisticModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAnnualStatisticAsync(model.Name, model.AnnualStatisticGroupId))
            {
                ModelState.AddModelError("AnnualStatistics", "That annual statistic already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsAnnualStatisticInUse(model.Id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This annual statistic is currently in use and cannot be edited." }
                });
            }

            var annualStatistics = new AnnualStatistic
            {
                AnnualStatisticId = model.Id,
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Name = model.Name
            };

            await _biobankWriteService.UpdateAnnualStatisticAsync(annualStatistics);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
                redirect = $"EditAnnualStatisticSuccess?name={model.Name}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteAnnualStatistic(AnnualStatisticModel model)
        {
            if (await _biobankReadService.IsAnnualStatisticInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The annual statistic \"{model.Name}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            var annualStatistic = new AnnualStatistic
            {
                AnnualStatisticId = model.Id,
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Name = model.Name
            };

            await _biobankWriteService.DeleteAnnualStatisticAsync(annualStatistic);

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The annual statistics type \"{model.Name}\" was deleted successfully.",
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