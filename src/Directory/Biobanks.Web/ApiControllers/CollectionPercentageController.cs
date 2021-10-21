using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.ADAC;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/CollectionPercentage")]
    public class CollectionPercentageController : ApiBaseController
    {
        private readonly IReferenceDataService<CollectionPercentage> _collectionPercentageService;

        public CollectionPercentageController(IReferenceDataService<CollectionPercentage> collectionPercentageService)
        {
            _collectionPercentageService = collectionPercentageService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _collectionPercentageService.List())
                .Select(x =>
                    Task.Run(async () => new CollectionPercentageModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        LowerBound = x.LowerBound,
                        UpperBound = x.UpperBound,
                        SampleSetsCount = await _collectionPercentageService.GetUsageCount(x.Id)
                    })
                    .Result
                )
                .ToList();

            return models;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(CollectionPercentageModel model)
        {
            // Validate model
            if (await _collectionPercentageService.Exists(model.Description))
            {
                ModelState.AddModelError("CollectionPercentage", "That description is already in use. Collection percentage descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var percentage = new CollectionPercentage
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound,
            };

            await _collectionPercentageService.Add(percentage);
            await _collectionPercentageService.Update(percentage);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, CollectionPercentageModel model)
        {
            // Validate model
            if (await _collectionPercentageService.Exists(model.Description))
            {
                ModelState.AddModelError("CollectionPercentage", "That collection percentage already exists!");
            }

            // If in use, prevent update
            if (model.SampleSetsCount > 0)
            {
                ModelState.AddModelError("CollectionPercentage", "That collection percentage is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _collectionPercentageService.Update(new CollectionPercentage
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
            var model = await _collectionPercentageService.Get(id);

            // If in use, prevent update
            if (await _collectionPercentageService.IsInUse(id))
            {
                ModelState.AddModelError("CollectionPercentage", $"The collection percentage \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _collectionPercentageService.Delete(id);

            // Success
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, CollectionPercentageModel model)
        {
            await _collectionPercentageService.Update(new CollectionPercentage
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });

        }

    }
}