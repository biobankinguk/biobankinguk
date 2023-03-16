using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Models.Submissions;
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
    public class AssociatedDataProcurementTimeFrameController : ControllerBase
    {
        private readonly IReferenceDataCrudService<AssociatedDataProcurementTimeframe> _associatedDataProcurementTimeFrameService;
        public AssociatedDataProcurementTimeFrameController(
        IReferenceDataCrudService<AssociatedDataProcurementTimeframe> associatedDataProcurementTimeFrameService)
        {
            _associatedDataProcurementTimeFrameService = associatedDataProcurementTimeFrameService;
        }

        /// <summary>
        /// Generate Associated Data Procurement TimeFrame
        /// </summary>
        /// <returns>A list of Associated Data Procurement TimeFrame</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(ReadAssociatedDataProcurementTimeFrameModel))]
        public async Task<IList> Get()
        {
            var models = (await _associatedDataProcurementTimeFrameService.List())
                .Select(x =>
                    Task.Run(async () => new ReadAssociatedDataProcurementTimeFrameModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        DisplayName = x.DisplayValue,
                        CollectionCapabilityCount = await _associatedDataProcurementTimeFrameService.GetUsageCount(x.Id),
                        SortOrder = x.SortOrder
                    })
                    .Result
                )
                .ToList();

            return models;
        }

        /// <summary>
        /// Delete an Associated Data Procurement TimeFrame.
        /// </summary>
        /// <param name="id">The ID of the condition to update.</param>
        /// <returns>Deleted Associated Data Procurement TimeFrame.</returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(AssociatedDataProcurementTimeFrameModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _associatedDataProcurementTimeFrameService.Get(id);

            //Validate min amount of time frames
            if (await _associatedDataProcurementTimeFrameService.Count() <= 2)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"A minimum amount of 2 time frames are allowed.");
            }

            if (await _associatedDataProcurementTimeFrameService.IsInUse(id))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"The associated data procurement time frame \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _associatedDataProcurementTimeFrameService.Delete(id);

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Update an Associated Data Procurement TimeFrame
        /// </summary>
        /// <param name="id">The ID of the condition to update.</param>
        /// <param name="model">The new model to update.</param>
        /// <returns>The updated Associated Data Procurement TimeFrame.</returns>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(AssociatedDataProcurementTimeFrameModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Put(int id, AssociatedDataProcurementTimeFrameModel model)
        {
            // Validate model
            var exisiting = await _associatedDataProcurementTimeFrameService.Get(model.Description);

            if (exisiting != null && exisiting.Id != id)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "That Associated Data Procurement Time Frame already exists!");
            }

            // If in use, prevent update
            if (await _associatedDataProcurementTimeFrameService.IsInUse(id))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"The Associated Data Procurement Time Frame \"{model.Description}\" is currently in use, and cannot be updated.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _associatedDataProcurementTimeFrameService.Update(new AssociatedDataProcurementTimeframe
            {
                Id = id,
                Value = model.Description,
                DisplayValue = model.DisplayName,
                SortOrder = model.SortOrder
            });

            // Success message
            return Ok(model);
        }

        /// <summary>
        /// Insert an Associated Data Procurement TimeFrame
        /// </summary>
        /// <param name="model">The new model to update.</param>
        /// <returns>The updated Associated Data Procurement TimeFrame.</returns>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(AssociatedDataProcurementTimeFrameModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Post(AssociatedDataProcurementTimeFrameModel model)
        {
            // Validate model
            if (await _associatedDataProcurementTimeFrameService.Count() >= 5)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"A maximum amount of 5 time frames are allowed.");
            }

            if (await _associatedDataProcurementTimeFrameService.Exists(model.Description))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "That description is already in use. Associated Data Procurement Time Frame descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var procurement = new AssociatedDataProcurementTimeframe
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                DisplayValue = model.DisplayName
            };

            await _associatedDataProcurementTimeFrameService.Add(procurement);
            await _associatedDataProcurementTimeFrameService.Update(procurement);

            // Success response
            return Ok(model);
        }

        /// <summary>
        /// Move an Associated Data Procurement TimeFrame
        /// </summary>
        /// <param name="id">The ID of the condition to move.</param>
        /// <param name="model">The new model to update.</param>
        /// <returns>The updated Associated Data Procurement TimeFrame.</returns>
        [HttpPost("{id}/move")]
        [SwaggerResponse(200, Type = typeof(AssociatedDataProcurementTimeFrameModel))]
        public async Task<IActionResult> Move(int id, AssociatedDataProcurementTimeFrameModel model)
        {
            await _associatedDataProcurementTimeFrameService.Update(new AssociatedDataProcurementTimeframe
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                DisplayValue = model.DisplayName
            });

            //Everything went A-OK!
            return Ok(model);
        }
    }
}
