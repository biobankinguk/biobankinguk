using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/MaterialTypeGroup")]
    public class MaterialTypeGroupController : ApiBaseController
    {
        private readonly IReferenceDataService<MaterialTypeGroup> _materialTypeGroupService;

        public MaterialTypeGroupController(IReferenceDataService<MaterialTypeGroup> materialTypeGroupService)
        {
            _materialTypeGroupService = materialTypeGroupService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IEnumerable> Get()
        {
            var materialTypeGroups = await _materialTypeGroupService.List();

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
        public async Task<IHttpActionResult> Post(MaterialTypeGroupModel model)
        {
            if (await _materialTypeGroupService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Material Type Group descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _materialTypeGroupService.Add(new MaterialTypeGroup
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
        public async Task<IHttpActionResult> Put(int id, MaterialTypeGroupModel model)
        {
            // Validate model
            if (await _materialTypeGroupService.Exists(model.Description))
            {
                ModelState.AddModelError("MaterialType", "That description is already in use. Material Type Groups must be unique.");
            }

            // If in use, prevent update
            if (await _materialTypeGroupService.IsInUse(id))
            {
                ModelState.AddModelError("MaterialType", $"The material type group \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _materialTypeGroupService.Update(new MaterialTypeGroup
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
            var model = await _materialTypeGroupService.Get(id);

            // If in use, prevent update
            if (await _materialTypeGroupService.IsInUse(id))
            {
                ModelState.AddModelError("MaterialType", $"The material type group \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _materialTypeGroupService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

    }
}