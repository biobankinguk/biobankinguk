using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Models.Shared;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Directory.Controllers.ReferenceData
{
    [Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class AssociatedDataTypeGroupController : ControllerBase
    {
        private readonly IReferenceDataCrudService<AssociatedDataTypeGroup> _associatedDataTypeGroupService;

        public AssociatedDataTypeGroupController(IReferenceDataCrudService<AssociatedDataTypeGroup> associatedDataTypeGroupService)
        {
            _associatedDataTypeGroupService = associatedDataTypeGroupService;
        }

        /// <summary>
        /// Generate a list of Associated Data Type Group.
        /// </summary>
        /// <returns>List of Associated Data Type Group.</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet("")]
        [AllowAnonymous]
        public async Task<IList> Get()
        {
            var model = (await _associatedDataTypeGroupService.List())
                .Select(x =>
                    Task.Run(async () => new ReadAssociatedDataTypeGroupModel
                    {
                        AssociatedDataTypeGroupId = x.Id,
                        Name = x.Value,
                        AssociatedDataTypeGroupCount = await _associatedDataTypeGroupService.GetUsageCount(x.Id)
                    })
                    .Result
                 )
                .ToList();

            return model;
        }

        /// <summary>
        /// Delete an Associated Data Type Group.
        /// </summary>
        /// <param name="id">Id of the model to delete.</param>
        /// <returns>The delete model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _associatedDataTypeGroupService.Get(id);

            if (await _associatedDataTypeGroupService.IsInUse(id))
            {
                ModelState.AddModelError("Name", $"The associated data type group \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _associatedDataTypeGroupService.Delete(id);

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Insert a new Associated Data Type Group.
        /// </summary>
        /// <param name="model">Model of new  Associated Data Type Group.</param>
        /// <returns>The inserted model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPost("")]
        public async Task<IActionResult> Post(AssociatedDataTypeGroupModel model)
        {
            //If this name is valid, it already exists
            if (await _associatedDataTypeGroupService.Exists(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Associated Data Type Group names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _associatedDataTypeGroupService.Add(new AssociatedDataTypeGroup
            {
                Value = model.Name
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Update an Associated Data Type Group.
        /// </summary>
        /// <param name="id">Id of the model to update.</param>
        /// <param name="model">Model with updates values.</param>
        /// <returns>The updated model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, AssociatedDataTypeGroupModel model)
        {
            var exisiting = await _associatedDataTypeGroupService.Get(model.Name);

            //If this name is valid, it already exists
            if (exisiting != null && exisiting.Id != id)
            {
                ModelState.AddModelError("Name", "That name is already in use by another asscoiated data type group. Associated Data Type Group names must be unique.");
            }

            if (await _associatedDataTypeGroupService.IsInUse(id))
            {
                ModelState.AddModelError("Name", "This associated data type group is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _associatedDataTypeGroupService.Update(new AssociatedDataTypeGroup
            {
                Id = id,
                Value = model.Name
            });

            //Everything went A-OK!
            return Ok(model);
        }
    }
}
