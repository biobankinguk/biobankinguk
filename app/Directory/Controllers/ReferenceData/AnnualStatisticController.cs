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
    public class AnnualStatisticController : ControllerBase
    {
        private readonly IReferenceDataCrudService<AnnualStatistic> _annualStatisticService;
        private readonly IReferenceDataCrudService<AnnualStatisticGroup> _annualStatisticGroupService;

        public AnnualStatisticController(
            IReferenceDataCrudService<AnnualStatistic> annualStatisticService,
            IReferenceDataCrudService<AnnualStatisticGroup> annualStatisticGroupService)
        {
            _annualStatisticService = annualStatisticService;
            _annualStatisticGroupService = annualStatisticGroupService;
        }
        /// <summary>
        /// Generate annual statistic list
        /// </summary>
        /// <returns>A list of annual statistics</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(AnnualStatisticModel))]
        public async Task<IList> Get()
        {
            var groups = (await _annualStatisticGroupService.List())
                 .Select(x => new AnnualStatisticGroupModel
                 {
                     AnnualStatisticGroupId = x.Id,
                     Name = x.Value,
                 })
                 .ToList();
            var models = (await _annualStatisticService.List())
                .Select(x =>
                    Task.Run(() => new AnnualStatisticModel
                    {
                        Id = x.Id,
                        Name = x.Value,
                        AnnualStatisticGroupId = x.AnnualStatisticGroupId,
                        AnnualStatisticGroupName = groups.Where(y => y.AnnualStatisticGroupId == x.AnnualStatisticGroupId).FirstOrDefault()?.Name,
                    })
                    .Result
                )
                .ToList();

            return models;
        }


        /// <summary>
        /// Insert an Annual Statistic.
        /// </summary>
        /// <param name="model">The model to be inserted.</param>
        /// <returns>The created Annual Statistic.</returns>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(AnnualStatisticModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Post(AnnualStatisticModel model)
        {
            var group = await _annualStatisticGroupService.Get(model.AnnualStatisticGroupId);

            // Validate model
            if (group.AnnualStatistics.Any(x => x.Value == model.Name))
            {
                ModelState.AddModelError("AnnualStatistics", "That name is already in use. Annual statistics names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var annualStatistic = new AnnualStatistic
            {
                Id = model.Id,
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Value = model.Name
            };

            await _annualStatisticService.Add(annualStatistic);

            // Success response
            return Ok(model);
        }

        /// <summary>
        /// Update an Annual Statistic.
        /// </summary>
        /// <param name="id">The ID of the condition to update.</param>
        /// <param name="model">The new model to be inserted.</param>
        /// <returns>The updated Annual Statistic.</returns>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(AnnualStatisticModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Put(int id, AnnualStatisticModel model)
        {
            var group = await _annualStatisticGroupService.Get(model.AnnualStatisticGroupId);

            // Validate model
            if (group.AnnualStatistics.Any(x => x.Value == model.Name))
            {
                ModelState.AddModelError("AnnualStatistics", "That annual statistic already exists!");
            }

            if (await _annualStatisticService.IsInUse(id))
            {
                ModelState.AddModelError("AnnualStatistics", "This annual statistic is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var annualStatistics = new AnnualStatistic
            {
                Id = id,
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Value = model.Name
            };

            await _annualStatisticService.Update(annualStatistics);

            // Success response
            return Ok(model);

        }

        /// <summary>
        /// Delete an Annual Statistic.
        /// </summary>
        /// <param name="id">The ID of the condition to update.</param>
        /// <returns>Deleted Annual Statistic.</returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(AnnualStatisticModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _annualStatisticService.Get(id);

            if (await _annualStatisticService.IsInUse(id))
            {
                ModelState.AddModelError("AnnualStatistics", $"The annual statistic \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _annualStatisticService.Delete(id);

            //Everything went A-OK!
            return Ok(model);

        }

        /// <summary>
        /// Move an Annual Statistic.
        /// </summary>
        /// <param name="id">The ID of the condition to move.</param>
        /// <param name="model">The new model to update.</param>
        /// <returns>The updated Annual Statistic.</returns>
        [HttpPost("{id}/move")]
        [SwaggerResponse(200, Type = typeof(AnnualStatisticModel))]
        public async Task<IActionResult> Move(int id, AnnualStatisticModel model)
        {
            var annualStatistics = new AnnualStatistic
            {
                Id = id,
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Value = model.Name
            };

            await _annualStatisticService.Update(annualStatistics);

            //Everything went A-OK!
            return Ok(model);
        }
    }
}
