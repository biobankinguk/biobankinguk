using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Web.Filters;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/MaterialTypeGroup")]
    public class MaterialTypeGroupController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public MaterialTypeGroupController(
            IBiobankReadService biobankReadService,
            IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IEnumerable> Get()
        {
            var materialTypeGroups = await _biobankReadService.ListMaterialTypeGroupsAsync();

            return materialTypeGroups.Select(x => new MaterialTypeGroupModel
            {
                Id = x.Id,
                Description = x.Value,
                MaterialTypes = x.MaterialTypes.Select(x => x.Value),
                MaterialTypeCount = x.MaterialTypes.Count()
            });
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(MaterialTypeModel model)
        {
            if (await _biobankReadService.ValidMaterialTypeGroupDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Material Type Group descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddMaterialTypeGroupAsync(new MaterialTypeGroup
            {
                Id = model.Id,
                Value = model.Description
            });

            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, MaterialTypeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidMaterialTypeDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("MaterialType", "That description is already in use. Material Type Groups must be unique.");
            }

            // If in use, prevent update
            if (await _biobankReadService.IsMaterialTypeGroupInUse(id))
            {
                ModelState.AddModelError("MaterialType", $"The material type group \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateMaterialTypeGroupAsync(new MaterialTypeGroup
            {
                Id = model.Id,
                Value = model.Description
            });

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
            var model = (await _biobankReadService.ListMaterialTypeGroupsAsync()).Where(x => x.Id == id).First();

            // If in use, prevent update
            if (await _biobankReadService.IsMaterialTypeGroupInUse(id))
            {
                ModelState.AddModelError("MaterialType", $"The material type group \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteMaterialTypeGroupAsync(new MaterialTypeGroup
            {
                Id = model.Id,
                Value = model.Value
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

    }
}