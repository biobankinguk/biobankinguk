using Biobanks.Entities.Shared.ReferenceData;
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
    public class SexController : ControllerBase
    {
        private readonly IReferenceDataCrudService<Sex> _sexService;

        public SexController(IReferenceDataCrudService<Sex> sexService)
        {
            _sexService = sexService;
        }

        /// <summary>
        /// Generate a list of Sexes
        /// </summary>
        /// <returns>List of Sexes</returns>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(ReadSexModel))]
        public async Task<IList> Get()
        {
            var model = (await _sexService.List())
                .Select(x =>
                    Task.Run(async () => new ReadSexModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SexCount = await _sexService.GetUsageCount(x.Id),
                        SortOrder = x.SortOrder
                    }).Result)
                .ToList();

            return model;
        }

        /// <summary>
        /// Insert a new Sex.
        /// </summary>
        /// <param name="model">New model to insert.</param>
        /// <returns>The inserted Sex.</returns>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(SexModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Post(SexModel model)
        {
            //If this description is valid, it already exists
            if (await _sexService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Sex descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _sexService.Add(new Sex
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Update an existing Sex.
        /// </summary>
        /// <param name="id">Id of the model to update.</param>
        /// <param name="model">Sex with new values.</param>
        /// <returns>The updated Sex.</returns>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(SexModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Put(int id, SexModel model)
        {
            var existing = await _sexService.Get(model.Description);

            //If this description is valid, it already exists
            if (existing != null && existing.Id != id)
            {
                ModelState.AddModelError("Description", "That description is already in use by another material type. Sex descriptions must be unique.");
            }

            if (await _sexService.IsInUse(id))
            {
                ModelState.AddModelError("Description", "This sex is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _sexService.Update(new Sex
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Delete an existing Sex.
        /// </summary>
        /// <param name="id">Id of the Sex to delete.</param>
        /// <returns>The deleted Sex.</returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(Sex))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _sexService.Get(id);

            if (await _sexService.IsInUse(id))
            {
                ModelState.AddModelError("Description", $"The sex \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _sexService.Delete(id);

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Move an existing Sex.
        /// </summary>
        /// <param name="id">Id of the Sex to move.</param>
        /// <param name="model">Model with updated values.</param>
        /// <returns></returns>
        [HttpPost("{id}/move")]
        [SwaggerResponse(200, Type = typeof(SexModel))]
        public async Task<ActionResult> Move(int id, SexModel model)
        {
            await _sexService.Update(new Sex
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

