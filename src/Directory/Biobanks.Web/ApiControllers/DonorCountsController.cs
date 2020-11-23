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
using Directory.Data.Constants;

namespace Biobanks.Web.ApiControllers
{
    public class DonorCountsController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public DonorCountsController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        // GET: DonorCounts
        [HttpGet]
        public async Task<IList> DonorCounts()
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
        public async Task<IHttpActionResult> AddDonorCountAjax(DonorCountModel model)
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
                redirect = $"AddDonorCountSuccess?name={model.Description}&referencename={currentReferenceName.Value}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditDonorCountAjax(DonorCountModel model, bool sortOnly = false)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.DonorCountName);

            // Validate model
            if (!sortOnly && await _biobankReadService.ValidDonorCountAsync(model.Description))
            {
                ModelState.AddModelError("DonorCounts", $"That {currentReferenceName.Value} already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = model.SampleSetsCount > 0;

            // Update Preservation Type
            await _biobankWriteService.UpdateDonorCountAsync(new DonorCount
            {
                DonorCountId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound
            },
            (sortOnly || inUse));

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditDonorCountSuccess?name={model.Description}&referencename={currentReferenceName.Value}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteDonorCount(DonorCountModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.DonorCountName);

            if (await _biobankReadService.IsDonorCountInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The {currentReferenceName.Value} \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteDonorCountAsync(new DonorCount
            {
                DonorCountId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = 0,
                UpperBound = 1
            });

            // Success
            return Json(new
            {
                msg = $"The {currentReferenceName.Value}  \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }
    }
}