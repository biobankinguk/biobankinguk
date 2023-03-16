using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Auth;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class CollectionStatusController : ControllerBase
    {
        private readonly IReferenceDataCrudService<CollectionStatus> _collectionStatusService;

        public CollectionStatusController(IReferenceDataCrudService<CollectionStatus> collectionStatusService)
        {
            _collectionStatusService = collectionStatusService;
        }

        /// <summary>
        /// Generate collection status list
        /// </summary>
        /// <returns>A list of collection status</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(ReadCollectionStatusModel))]
        public async Task<IList> Get()
        {
            var model = (await _collectionStatusService.List())
                    .Select(x =>

                Task.Run(async () => new ReadCollectionStatusModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    CollectionCount = await _collectionStatusService.GetUsageCount(x.Id),
                    SortOrder = x.SortOrder
                }).Result)

            .ToList();

            return model;
        }

        /// <summary>
        /// Delete a collection status.
        /// </summary>
        /// <param name="id">The ID of the collection to update.</param>
        /// <returns>Deleted  collection status.</returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(ReadCollectionStatusModel))]
        [SwaggerResponse(400, "Invalid request body.")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _collectionStatusService.Get(id);

            // If in use, prevent update
            if (await _collectionStatusService.IsInUse(id))
            {
                ModelState.AddModelError("CollectionStatus", $"The collection status \"{model.Value}\" is currently in use, and cannot be deleted.");
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _collectionStatusService.Delete(id);

            //Everything went A-OK!
            return Ok(model);

        }

        /// <summary>
        /// Update a collection status.
        /// </summary>
        /// <param name="id">The ID of the collection to update.</param>
        /// <param name="model">The new model to be inserted.</param>
        /// <returns>The updated collection status.</returns>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(ReadCollectionStatusModel))]
        [SwaggerResponse(400, "Invalid request body.")]
        public async Task<IActionResult> Put(int id, CollectionStatusModel model)
        {
            // Validate model
            if (await _collectionStatusService.Exists(model.Description))
            {
                ModelState.AddModelError("CollectionStatus", "That collection status already exists!");
            }

            // If in use, prevent update
            if (await _collectionStatusService.IsInUse(id))
            {
                ModelState.AddModelError("CollectionStatus", $"The collection status \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _collectionStatusService.Update(new CollectionStatus
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);

        }

        /// <summary>
        /// Insert a collection status.
        /// </summary>
        /// <param name="model">The model to be inserted.</param>
        /// <returns>The created collection status.</returns>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(ReadCollectionStatusModel))]
        [SwaggerResponse(400, "Invalid request body.")]
        public async Task<IActionResult> Post(CollectionStatusModel model)
        {
            //If this description is valid, it already exists
            if (await _collectionStatusService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Collection status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _collectionStatusService.Add(new CollectionStatus
            {
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);

        }

        /// <summary>
        /// Move a collection status.
        /// </summary>
        /// <param name="id">The ID of the collection  to move.</param>
        /// <param name="model">The new model to update.</param>
        /// <returns>The updated collection status.</returns>
        [HttpPost("{id}/move")]
        [SwaggerResponse(200, Type = typeof(ReadCollectionStatusModel))]
        public async Task<IActionResult> Move(int id, CollectionStatusModel model)
        {
            await _collectionStatusService.Update(new CollectionStatus
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
