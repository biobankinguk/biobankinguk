using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Directory.Data.Constants;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/DonorCount")]
    public class DonorCountController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public DonorCountController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _biobankReadService.ListDonorCountsAsync(true))
                .Select(x =>
                    Task.Run(async () => new DonorCountModel()
                    {
                        Id = x.DonorCountId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        LowerBound = x.LowerBound,
                        UpperBound = x.UpperBound,
                        SampleSetsCount = await _biobankReadService.GetDonorCountUsageCount(x.DonorCountId)
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
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.DonorCountName);

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
                DonorCountId = model.Id,
                Description = model.Description,
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
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.DonorCountName);

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
                DonorCountId = id,
                Description = model.Description,
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
            var model = (await _biobankReadService.ListDonorCountsAsync()).Where(x => x.DonorCountId == id).First();

            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.DonorCountName);

            // If in use, prevent update
            if (await _biobankReadService.IsDonorCountInUse(id))
            {
                ModelState.AddModelError("DonorCounts", $"The donor count \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteDonorCountAsync(new DonorCount
            {
                DonorCountId = id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = 0,
                UpperBound = 1
            });

            // Success
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, DonorCountModel model)
        {
            await _biobankWriteService.UpdateDonorCountAsync(new DonorCount
            {
                DonorCountId = id,
                Description = model.Description,
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