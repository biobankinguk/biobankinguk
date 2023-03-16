using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
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
    public class CollectionPercentageController : ControllerBase
    {
        private readonly IReferenceDataCrudService<CollectionPercentage> _collectionPercentageService;

        public CollectionPercentageController(IReferenceDataCrudService<CollectionPercentage> collectionPercentageService)
        {
            _collectionPercentageService = collectionPercentageService;
        }

        /// <summary>
        /// Generate a Collection Percentage list
        /// </summary>
        /// <returns>A list of Collection Percentage</returns>
        [HttpGet]
        [AllowAnonymous]
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

        /// <summary>
        /// Insert new Collection Percentag.
        /// </summary>
        /// <param name="model">Model of the collection percentage to insert.</param>
        /// <returns>Model of the inserted collection percentage .</returns>
        /// <response code="200">Request Accepted</response>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(CollectionPercentageModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Post(CollectionPercentageModel model)
        {
            // Validate model
            if (await _collectionPercentageService.ExistsExcludingId(model.Id, model.Description))
            {
                ModelState.AddModelError("CollectionPercentage", "That description is already in use. Collection percentage descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
            return Ok(model);
        }

        /// <summary>
        /// Update existing collection percentage.
        /// </summary>
        /// <param name="id">ID of the collection percentage to update.</param>
        /// <param name="model">Model with new values.</param>
        /// <returns>Model of the updated collection percentage.</returns>
        /// <response code="200">Request Accepted</response>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(CollectionPercentageModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Put(int id, CollectionPercentageModel model)
        {
            // Validate model
            if (await _collectionPercentageService.ExistsExcludingId(model.Id, model.Description))
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
                return BadRequest(ModelState);
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
            return Ok(model);

        }

        /// <summary>
        /// Delete collection percentage.
        /// </summary>
        /// <param name="id">ID of the collection percentage to delete.</param>
        /// <returns>Model of the deleted collection percentage.</returns>
        /// <response code="200">Request Accepted</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(CollectionPercentageModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _collectionPercentageService.Get(id);

            // If in use, prevent update
            if (await _collectionPercentageService.IsInUse(id))
            {
                ModelState.AddModelError("CollectionPercentage", $"The collection percentage \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            await _collectionPercentageService.Delete(id);

            // Success
            return Ok(model);

        }

        /// <summary>
        /// Move a collection percentage.
        /// </summary>
        /// <param name="id">The ID of the collection percentage to move.</param>
        /// <param name="model">The new model to update.</param>
        /// <returns>The updated collection percentage.</returns>
        [HttpPost("{id}/move")]
        [SwaggerResponse(200, Type = typeof(CollectionPercentageModel))]
        public async Task<IActionResult> Move(int id, CollectionPercentageModel model)
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
            return Ok(model);

        }

    }
}
