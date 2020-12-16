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
    [RoutePrefix("api/AnnualStatistics")]
    public class AnnualStatisticsController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AnnualStatisticsController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }


        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Get()
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
        [Route("")]
        public async Task<IHttpActionResult> Post(AnnualStatisticModel model)
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
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, AnnualStatisticModel model)
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

            if (await _biobankReadService.IsAnnualStatisticInUse(id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This annual statistic is currently in use and cannot be edited." }
                });
            }

            var annualStatistics = new AnnualStatistic
            {
                AnnualStatisticId = id,
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Name = model.Name
            };

            await _biobankWriteService.UpdateAnnualStatisticAsync(annualStatistics);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
            });
        }

        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListAnnualStatisticsAsync()).Where(x => x.AnnualStatisticId == id).First();

            if (await _biobankReadService.IsAnnualStatisticInUse(id))
            {
                return Json(new
                {
                    msg = $"The annual statistic \"{model.Name}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            var annualStatistic = new AnnualStatistic
            {
                AnnualStatisticId = id,
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

        [HttpPut]
        [Route("Sort/{id}")]
        public async Task<IHttpActionResult> Sort(int id, AnnualStatisticModel model)
        {

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var annualStatistics = new AnnualStatistic
            {
                AnnualStatisticId = id,
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Name = model.Name
            };

            await _biobankWriteService.UpdateAnnualStatisticAsync(annualStatistics, true);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });

        }
    }
}