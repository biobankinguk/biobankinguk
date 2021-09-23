using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/MaterialType")]
    public class MaterialTypeController : ApiBaseController
    {
        private readonly IReferenceDataService<MaterialType> _materialTypeService;
        private readonly IBiobankReadService _biobankReadService;

        public MaterialTypeController(
            IReferenceDataService<MaterialType> materialTypeService,
            IBiobankReadService biobankReadService)
        {
            _materialTypeService = materialTypeService;
            _biobankReadService = biobankReadService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _materialTypeService.List())
                    .Select(x =>

                    Task.Run(async () => new ReadMaterialTypeModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        MaterialDetailCount = await _biobankReadService.GetMaterialTypeMaterialDetailCount(x.Id),
                        SortOrder = x.SortOrder
                    }).Result).ToList();

            return model;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(MaterialTypeModel model)
        {
            //If this description is valid, it already exists
            if (await _materialTypeService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Disease status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _materialTypeService.Add(new MaterialType
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
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
            if (await _materialTypeService.Exists(model.Description))
            {
                ModelState.AddModelError("MaterialType", "That description is already in use. Material types must be unique.");
            }

            // If in use, prevent update
            if (await _materialTypeService.IsInUse(id))
            {
                ModelState.AddModelError("MaterialType", $"The material type \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _materialTypeService.Update(new MaterialType
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder
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
            var model = await _materialTypeService.Get(id);

            // If in use, prevent update
            if (await _materialTypeService.IsInUse(id))
            {
                ModelState.AddModelError("MaterialType", $"The material type \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _materialTypeService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, MaterialTypeModel model)
        {
            await _materialTypeService.Update(new MaterialType
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{materialType}/extractionprocedure")]
        public async Task<IList> GetValidExtractionProcedures(int materialType)
            => (await _biobankReadService.GetMaterialTypeExtractionProcedures(materialType,true)).Select(x => new { x.Id, x.Value }).ToList();
    }
}