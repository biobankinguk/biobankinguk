using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Auth;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class RegistrationReasonController : ControllerBase
    {
        private readonly IReferenceDataCrudService<RegistrationReason> _registrationReasonService;

        public RegistrationReasonController(IReferenceDataCrudService<RegistrationReason> registrationReasonService)
        {
            _registrationReasonService = registrationReasonService;
        }

        /// <summary>
        /// Generate a list of Registration Reasons.
        /// </summary>
        /// <returns>The list of Registration Reasons.</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(ReadRegistrationReasonModel))]
        public async Task<IList> Get()
        {
            var model = (await _registrationReasonService.List())
                .Select(x =>

            Task.Run(async () => new ReadRegistrationReasonModel
            {
                Id = x.Id,
                Description = x.Value,
                OrganisationCount = await _registrationReasonService.GetUsageCount(x.Id),
            }).Result)

                .ToList();

            return model;
        }

        /// <summary>
        /// Insert a new Registration Reason.
        /// </summary>
        /// <param name="model">The model to insert.</param>
        /// <returns>The inserted Registration Reason.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(RegistrationReason))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Post(RegistrationReasonModel model)
        {
            //If this description is valid, it already exists
            if (await _registrationReasonService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Registration reason descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _registrationReasonService.Add(new RegistrationReason
            {
                Value = model.Description
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Update a Registration Reason.
        /// </summary>
        /// <param name="id">Id of the Registration Reason.</param>
        /// <param name="model">Model with updated values.</param>
        /// <returns>The updated Registration Reason.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(RegistrationReason))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Put(int id, RegistrationReasonModel model)
        {
            var existing = await _registrationReasonService.Get(model.Description);

            //If this description is valid, it already exists
            if (existing != null && existing.Id != id)
            {
                ModelState.AddModelError("Description", "That description is already in use by another registration reason. Registration reason descriptions must be unique.");
            }

            if (await _registrationReasonService.IsInUse(id))
            {
                ModelState.AddModelError("Description", "This registration reason is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _registrationReasonService.Update(new RegistrationReason
            {
                Id = id,
                Value = model.Description
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Delete a Registration Reason.
        /// </summary>
        /// <param name="id">Id of the Registration Reason to delete.</param>
        /// <returns>The deleted Registration Reason.</returns>
        /// <response code="200">Request Successful</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(RegistrationReason))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _registrationReasonService.Get(id);

            if (await _registrationReasonService.IsInUse(id))
            {
                ModelState.AddModelError("Description", $"The registration reason \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _registrationReasonService.Delete(id);

            //Everything went A-OK!
            return Ok(model);
        }
    }
}
