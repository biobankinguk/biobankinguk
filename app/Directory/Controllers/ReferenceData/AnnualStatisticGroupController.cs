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
    public class AnnualStatisticGroupController : ControllerBase
    {
        private readonly IReferenceDataCrudService<AnnualStatisticGroup> _annualStatisticGroupService;

        public AnnualStatisticGroupController(IReferenceDataCrudService<AnnualStatisticGroup> annualStatisticGroupService)
        {
            _annualStatisticGroupService = annualStatisticGroupService;
        }

        /// <summary>
        /// Generate annual statistic group list
        /// </summary>
        /// <returns>A list of annual statistic groups</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(AnnualStatisticGroupModel))]
        public async Task<IList> Get()
        {
            var model = (await _annualStatisticGroupService.List())
                .Select(x =>

                Task.Run(() => Task.FromResult(new AnnualStatisticGroupModel
                {
                    AnnualStatisticGroupId = x.Id,
                    Name = x.Value,
                })).Result)

                .ToList();

            return model;
        }

        /// <summary>
        /// Insert new annual statistic group.
        /// </summary>
        /// <param name="model">Model of the group to insert.</param>
        /// <returns>Model of the inserted group.</returns>
        /// <response code="200">Request Accepted</response>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(AnnualStatisticGroupModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Post(AnnualStatisticGroupModel model)
        {
            //If this name is valid, it already exists
            if (await _annualStatisticGroupService.Exists(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Annual Statistic Group names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _annualStatisticGroupService.Add(new AnnualStatisticGroup
            {
                Value = model.Name
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Update existing annual statistic group.
        /// </summary>
        /// <param name="id">ID of the group to update.</param>
        /// <param name="model">Model with new values.</param>
        /// <returns>Model of the updated group.</returns>
        /// <response code="200">Request Accepted</response>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(AnnualStatisticGroupModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Put(int id, AnnualStatisticGroupModel model)
        {
            var existing = await _annualStatisticGroupService.Get(model.Name);

            //If this name is valid, it already exists
            if (existing != null && existing.Id != id)
            {
                ModelState.AddModelError("Name", "That name is already in use by another annual statistic group. Annual Statistic Group names must be unique.");
            }

            if (await _annualStatisticGroupService.IsInUse(id))
            {
                ModelState.AddModelError("Name", "This annual statistic group is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _annualStatisticGroupService.Update(new AnnualStatisticGroup
            {
                Id = model.AnnualStatisticGroupId,
                Value = model.Name
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Delete annual statistic group.
        /// </summary>
        /// <param name="id">ID of the group to delete.</param>
        /// <returns>Model of the deleted group.</returns>
        /// <response code="200">Request Accepted</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(AnnualStatisticGroupModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _annualStatisticGroupService.Get(id);

            if (await _annualStatisticGroupService.IsInUse(id))
            {
                ModelState.AddModelError("Name", "This annual statistic group is currently in use and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _annualStatisticGroupService.Delete(id);

            //Everything went A-OK!
            return Ok(model);
        }
    }
}
