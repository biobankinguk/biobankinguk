using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class MaterialTypeController : ControllerBase
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
        [AllowAnonymous]
        public async Task<ActionResult> Post(MaterialTypeModel model)
        {
            //If this description is valid, it already exists
            if (await _materialTypeService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Disease status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _materialTypeService.Add(new MaterialType
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Accepted(model);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> Put(int id, MaterialTypeModel model)
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
                return BadRequest(ModelState);
            }

            await _materialTypeService.Update(new MaterialType
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            return Accepted(model);
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _materialTypeService.Get(id);

            // If in use, prevent update
            if (await _materialTypeService.IsInUse(id))
            {
                ModelState.AddModelError("MaterialType", $"The material type \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _materialTypeService.Delete(id);

            //Everything went A-OK!
            return Accepted(model);
        }

        [HttpPost("{id}/move")]
        [AllowAnonymous]
        public async Task<ActionResult> Move(int id, MaterialTypeModel model)
        {
            await _materialTypeService.Update(new MaterialType
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Accepted(model);
        }

        [HttpGet("{materialType}/extractionprocedure")]
        [AllowAnonymous]
        public async Task<IList> GetValidExtractionProcedures(int materialType)
            => (await _biobankReadService.GetMaterialTypeExtractionProcedures(materialType, true)).Select(x => new { x.Id, x.Value }).ToList();
    }
}
