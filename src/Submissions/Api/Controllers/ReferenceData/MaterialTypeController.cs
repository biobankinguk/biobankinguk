using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.Directory;
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
        private readonly IMaterialService _materialTypeService;

        public MaterialTypeController(
            IMaterialService materialTypeService)
        {
            _materialTypeService = materialTypeService;
        }

        /// <summary>
        /// Generate a list of Material Types.
        /// </summary>
        /// <returns>List of material types.</returns>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(MaterialType))]
        [SwaggerResponse(400, "Invalid request")]
        public async Task<IList> Get()
        {
            var model = (await _materialTypeService.List())
                    .Select(x =>

                    Task.Run(async () => new ReadMaterialTypeModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        MaterialDetailCount = await _materialTypeService.GetDetailCount(x.Id),
                        SortOrder = x.SortOrder
                    }).Result).ToList();

            return model;
        }

        /// <summary>
        /// Insert a new Material Type.
        /// </summary>
        /// <param name="model">Model of Material Type to insert.</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(MaterialType))]
        [SwaggerResponse(400, "Invalid request")]
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
            return Ok(model);
        }

        /// <summary>
        /// Update a Material Type.
        /// </summary>
        /// <param name="id">Id of the Material Type to update.</param>
        /// <param name="model">Model with new values.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(MaterialType))]
        [SwaggerResponse(400, "Invalid request")]
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

            return Ok(model);
        }

        /// <summary>
        /// Deleta a Material Type.
        /// </summary>
        /// <param name="id">Id of the Material Type to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(MaterialType))]
        [SwaggerResponse(400, "Invalid request")]
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
            return Ok(model);
        }

        /// <summary>
        /// Move a Material Type.
        /// </summary>
        /// <param name="id">Id of the Material Type to move.</param>
        /// <param name="model">Model with updated values.</param>
        /// <returns></returns>
        [HttpPost("{id}/move")]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(MaterialType))]
        public async Task<ActionResult> Move(int id, MaterialTypeModel model)
        {
            await _materialTypeService.Update(new MaterialType
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="materialType"></param>
        /// <returns></returns>
        [HttpGet("{materialType}/extractionprocedure")]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(MaterialType))]
        public async Task<IList> GetValidExtractionProcedures(int materialType)
            => (await _materialTypeService.GetMaterialTypeExtractionProcedures(materialType, true)).Select(x => new { x.Id, x.Value }).ToList();
    }
}
