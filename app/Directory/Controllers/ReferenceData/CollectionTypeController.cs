using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Models.Shared;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.Directory.Controllers.ReferenceData
{
    [Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class CollectionTypeController : ControllerBase
    {
        private readonly IReferenceDataCrudService<CollectionType> _collectionTypeService;

        public CollectionTypeController(IReferenceDataCrudService<CollectionType> collectionTypeService)
        {
            _collectionTypeService = collectionTypeService;
        }


        /// <summary>
        /// Generate collection type list
        /// </summary>
        /// <returns>A list of collection type </returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(ReadCollectionTypeModel))]
        public async Task<IList> Get()
        {
            var model = (await _collectionTypeService.List())
                    .Select(x =>

                Task.Run(async () => new ReadCollectionTypeModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    CollectionCount = await _collectionTypeService.GetUsageCount(x.Id),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList();

            return model;
        }

        /// <summary>
        /// Delete a collection type.
        /// </summary>
        /// <param name="id">The ID of the collection type to update.</param>
        /// <returns>Deleted collection type.</returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(CollectionTypeModel))]
        [SwaggerResponse(400, "Invalid request body.")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _collectionTypeService.Get(id);

            // If in use, prevent update
            if (await _collectionTypeService.IsInUse(id))
            {
                ModelState.AddModelError("CollectionType", $"The Collection type \"{model.Value}\" is currently in use, and cannot be deleted.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _collectionTypeService.Delete(id);

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Update a collection type.
        /// </summary>
        /// <param name="id">The ID of the collection to update.</param>
        /// <param name="model">The new model to be inserted.</param>
        /// <returns>The updated collection type .</returns>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(CollectionTypeModel))]
        [SwaggerResponse(400, "Invalid request body.")]
        public async Task<IActionResult> Put(int id, CollectionTypeModel model)
        {
            // Validate model
            if (await _collectionTypeService.Exists(model.Description))
            {
                ModelState.AddModelError("CollectionType", "That collection type already exists!");
            }

            // If in use, prevent update
            if (await _collectionTypeService.IsInUse(id))
            {
                ModelState.AddModelError("CollectionType", $"The Collection type \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _collectionTypeService.Update(new CollectionType
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Insert a collection type.
        /// </summary>
        /// <param name="model">The model to be inserted.</param>
        /// <returns>The created collection type.</returns>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(CollectionTypeModel))]
        [SwaggerResponse(400, "Invalid request body.")]
        public async Task<IActionResult> Post(CollectionTypeModel model)
        {
            //If this description is valid, it already exists
            if (await _collectionTypeService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Collection types descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _collectionTypeService.Add(new CollectionType
            {
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Move a collection type.
        /// </summary>
        /// <param name="id">The ID of the a collection to move.</param>
        /// <param name="model">The new model to update.</param>
        /// <returns>The updated collection type.</returns>
        [HttpPost("{id}/move")]
        [SwaggerResponse(200, Type = typeof(CollectionTypeModel))]
        public async Task<IActionResult> Move(int id, CollectionTypeModel model)
        {
            await _collectionTypeService.Update(new CollectionType
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);
        }

    }
}
