using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Directory.Data.Constants;
using Biobanks.Web.Filters;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/DonorCount")]
    public class DonorCountController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;
        private readonly IConfigService _configService;

        public DonorCountController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService, IConfigService configService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
            _configService = configService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _biobankReadService.ListDonorCountsAsync(true))
                .Select(x =>
                    Task.Run(async () => new DonorCountModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        LowerBound = x.LowerBound,
                        UpperBound = x.UpperBound,
                        SampleSetsCount = await _biobankReadService.GetDonorCountUsageCount(x.Id)
                    })
                    .Result
                )
                .ToList();

            return models;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(DonorCountModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.DonorCountName);

            // Validate model
            if (await _biobankReadService.ValidDonorCountAsync(model.Description))
            {
                ModelState.AddModelError("DonorCounts", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var donor = new DonorCount
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound

            };

            await _biobankWriteService.AddDonorCountAsync(donor);
            await _biobankWriteService.UpdateDonorCountAsync(donor, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, DonorCountModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.DonorCountName);

            // Validate model
            if (await _biobankReadService.ValidDonorCountAsync(model.Description))
            {
                ModelState.AddModelError("DonorCounts", $"That {currentReferenceName.Value} already exists!");
            }

            // If in use, prevent update
            if (model.SampleSetsCount > 0)
            {
                ModelState.AddModelError("DonorCounts", $"The donor count \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateDonorCountAsync(new DonorCount
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound
            });

            // Success message
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
            var model = (await _biobankReadService.ListDonorCountsAsync(true)).Where(x => x.Id == id).First();

            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.DonorCountName);

            // If in use, prevent update
            if (await _biobankReadService.IsDonorCountInUse(id))
            {
                ModelState.AddModelError("DonorCounts", $"The donor count \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteDonorCountAsync(new DonorCount
            {
                Id = id,
                Value = model.Value,
                SortOrder = model.SortOrder,
                LowerBound = 0,
                UpperBound = 1
            });

            // Success
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, DonorCountModel model)
        {
            await _biobankWriteService.UpdateDonorCountAsync(new DonorCount
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound
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