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
    public class CollectionTypeController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public CollectionTypeController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        // GET: CollectionType
        [HttpGet]
        public async Task<IList> CollectionType()
        {
            var model = (await _biobankReadService.ListCollectionTypesAsync())
                    .Select(x =>

                Task.Run(async () => new ReadCollectionTypeModel
                {
                    Id = x.CollectionTypeId,
                    Description = x.Description,
                    CollectionCount = await _biobankReadService.GetCollectionTypeCollectionCount(x.CollectionTypeId),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList();

            return model;
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteCollectionType(Models.Shared.CollectionTypeModel model)
        {
            if (await _biobankReadService.IsCollectionTypeInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The collection type \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteCollectionTypeAsync(new CollectionType
            {
                CollectionTypeId = model.Id,
                Description = model.Description
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The collection type \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });

        }

        [HttpPost]
        public async Task<IHttpActionResult> EditCollectionTypeAjax(Models.Shared.CollectionTypeModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidCollectionTypeDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("CollectionType", "That collection type already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }
            // If in use, then only re-order the type
            bool inUse = await _biobankReadService.IsCollectionTypeInUse(model.Id);

            // Update Preservation Type
            await _biobankWriteService.UpdateCollectionTypeAsync(new CollectionType
            {
                CollectionTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddCollectionTypeAjax(Models.Shared.CollectionTypeModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidCollectionTypeDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Collection types descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddCollectionTypeAsync(new CollectionType
            {
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }
    }
}