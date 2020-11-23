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
    public class MaterialTypesController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public MaterialTypesController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        public async Task<IList> MaterialTypes()
        {
            var model = (await _biobankReadService.ListMaterialTypesAsync())
                    .Select(x =>

                    Task.Run(async () => new ReadMaterialTypeModel
                    {
                        Id = x.MaterialTypeId,
                        Description = x.Description,
                        MaterialDetailCount = await _biobankReadService.GetMaterialTypeMaterialDetailCount(x.MaterialTypeId),
                        SortOrder = x.SortOrder
                    }).Result).ToList();

            return model;
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddMaterialTypeAjax(MaterialTypeModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidMaterialTypeDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Disease status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddMaterialTypeAsync(new MaterialType
            {
                MaterialTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddMaterialTypeSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditMaterialTypeAjax(MaterialTypeModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidMaterialTypeDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("MaterialType", "That description is already in use. Material types must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = false;

            // Update Preservation Type
            await _biobankWriteService.UpdateMaterialTypeAsync(new MaterialType
            {
                MaterialTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditMaterialTypeSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteMaterialType(MaterialTypeModel model)
        {
            if (await _biobankReadService.IsMaterialTypeInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The material type \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteMaterialTypeAsync(new MaterialType
            {
                MaterialTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The material type \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }
    }
}