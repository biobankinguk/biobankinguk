using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Web.Http;
using System.Threading.Tasks;
using Biobanks.Services.Contracts;
using System.Linq;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/AnnualStatistic")]
    public class AnnualStatisticController : ApiBaseController
    {
        private readonly IReferenceDataService<AnnualStatistic> _annualStatisticService;

        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AnnualStatisticController(
            IReferenceDataService<AnnualStatistic> annualStatisticService,
            IBiobankReadService biobankReadService,
            IBiobankWriteService biobankWriteService)
        {
            _annualStatisticService = annualStatisticService;
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            var groups = (await _biobankReadService.ListAnnualStatisticGroupsAsync())
                 .Select(x => new AnnualStatisticGroupModel
                 {
                     AnnualStatisticGroupId = x.Id,
                     Name = x.Value,
                 })
                 .ToList();

            var models = (await _annualStatisticService.List())
                .Select(x =>
                    Task.Run(async () => new AnnualStatisticModel
                    {
                        Id = x.Id,
                        Name = x.Value,
                        UsageCount = await _annualStatisticService.GetUsageCount(x.Id),
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
            if (await _biobankReadService.Exists(model.Name, model.AnnualStatisticGroupId))
            {
                ModelState.AddModelError("AnnualStatistics", "That name is already in use. Annual statistics names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var annualStatistic = new AnnualStatistic
            {
                Id = model.Id,
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Value = model.Name
            };

            await _annualStatisticService.Add(annualStatistic);

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

            if (await _annualStatisticService.IsInUse(id))
            {
                ModelState.AddModelError("AnnualStatistics", "This annual statistic is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var annualStatistics = new AnnualStatistic
            {
                Id = id,
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Value = model.Name
            };

            await _annualStatisticService.Update(annualStatistics);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = await _annualStatisticService.Get(id);

            if (await _annualStatisticService.IsInUse(id))
            {
                ModelState.AddModelError("AnnualStatistics", $"The annual statistic \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _annualStatisticService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value
            });

        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, AnnualStatisticModel model)
        {

            var annualStatistics = new AnnualStatistic
            {
                Id = id,
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Value = model.Name
            };

            await _annualStatisticService.Update(annualStatistics);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });

        }
    }
}