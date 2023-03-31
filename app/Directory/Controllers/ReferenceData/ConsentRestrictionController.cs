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
    public class ConsentRestrictionController : ControllerBase
    {
        private readonly IReferenceDataCrudService<ConsentRestriction> _consentRestrictionService;

        public ConsentRestrictionController(IReferenceDataCrudService<ConsentRestriction> consentRestrictionService)
        {
            _consentRestrictionService = consentRestrictionService;
        }

        /// <summary>
        /// Generate a consent restriction list.
        /// </summary>
        /// <returns>List of consent restriction.</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(ReadConsentRestrictionModel))]
        public async Task<IList> Get()
        {
            var model = (await _consentRestrictionService.List())
                    .Select(x =>

                Task.Run(async () => new ReadConsentRestrictionModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    CollectionCount = await _consentRestrictionService.GetUsageCount(x.Id),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList();

            return model;
        }

        /// <summary>
        /// Delete existing consent restriction.
        /// </summary>
        /// <param name="id">ID of the  conesent restriction to delete.</param>
        /// <returns>The deleted consent restrict.</returns>
        /// <response code="200">Request Accepted</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(ConsentRestrictionModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _consentRestrictionService.Get(id);

            // If in use, prevent update
            if (await _consentRestrictionService.IsInUse(id))
            {
                ModelState.AddModelError("ConsentRestriction", $"The consent restriction \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _consentRestrictionService.Delete(id);

            //Everything went A-OK!
            return Ok(model);

        }

        /// <summary>
        /// Update existing consent restrict.
        /// </summary>
        /// <param name="id">ID of the consent restriction to update.</param>
        /// <param name="model">Model of the values to update with.</param>
        /// <returns>The updated preservation type model.</returns>
        /// <response code="200">Request Accepted</response>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(ConsentRestrictionModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Put(int id, ConsentRestrictionModel model)
        {
            // Validate model
            if (await _consentRestrictionService.Exists(model.Description))
            {
                ModelState.AddModelError("ConsentRestriction", "That consent restriction already exists!");
            }

            // If in use, prevent update
            if (await _consentRestrictionService.IsInUse(id))
            {
                ModelState.AddModelError("ConsentRestriction", $"The consent restriction \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _consentRestrictionService.Update(new ConsentRestriction
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);

        }
        /// <summary>
        /// Insert new consent restriction.
        /// </summary>
        /// <param name="model">Model of consent restriction to insert.</param>
        /// <returns></returns>
        /// <response code="200">Request Accepted</response>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(ConsentRestrictionModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Post(ConsentRestrictionModel model)
        {
            //If this description is valid, it already exists
            if (await _consentRestrictionService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Consent restriction descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _consentRestrictionService.Add(new ConsentRestriction
            {
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Move an existing consent restriction.
        /// </summary>
        /// <param name="id">ID of the consent restriction to move.</param>
        /// <param name="model">Model of the values to update with.</param>
        /// <returns>The updated consent restriction.</returns>
        /// <response code="200">Request Accepted</response>
        [HttpPost("{id}/move")]
        [SwaggerResponse(200, Type = typeof(ConsentRestrictionModel))]
        public async Task<IActionResult> Move(int id, ConsentRestrictionModel model)
        {
            await _consentRestrictionService.Update(new ConsentRestriction
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
