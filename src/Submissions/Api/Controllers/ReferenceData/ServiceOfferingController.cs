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
    public class ServiceOfferingController : ControllerBase
    {
        private readonly IReferenceDataCrudService<ServiceOffering> _serviceOfferingService;

        public ServiceOfferingController(IReferenceDataCrudService<ServiceOffering> serviceOfferingService)
        {
            _serviceOfferingService = serviceOfferingService;
        }

        /// <summary>
        /// Generate a list of Service Offering.
        /// </summary>
        /// <returns>The list of Service Offerings.</returns>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(ServiceOffering))]
        public async Task<IList> Get()
        {
            var models = (await _serviceOfferingService.List())
                .Select(x =>

            Task.Run(async () => new ReadServiceOfferingModel
            {
                Id = x.Id,
                Name = x.Value,
                OrganisationCount = await _serviceOfferingService.GetUsageCount(x.Id),
                SortOrder = x.SortOrder
            }).Result)

                .ToList();

            return models;
        }

        /// <summary>
        /// Insert a new Service Offering.
        /// </summary>
        /// <param name="model">Model to insert.</param>
        /// <returns>The inserted model.</returns>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(ServiceOffering))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Post(ServiceOfferingModel model)
        {
            //If this description is valid, it already exists
            if (await _serviceOfferingService.Exists(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Service offering names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _serviceOfferingService.Add(new ServiceOffering
            {
                Value = model.Name,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Update an existing Service Offering.
        /// </summary>
        /// <param name="id">Id of the model to update.</param>
        /// <param name="model">The model with updated values.</param>
        /// <returns>The updated model.</returns>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(ServiceOffering))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Put(int id, ServiceOfferingModel model)
        {
            // Validate model
            if (await _serviceOfferingService.Exists(model.Name))
            {
                ModelState.AddModelError("ServiceOffering", "That service offering already exists!");
            }

            // If in use, prevent update
            if (await _serviceOfferingService.IsInUse(id))
            {
                ModelState.AddModelError("ServiceOffering", $"The service offering \"{model.Name}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Update Service Offering
            await _serviceOfferingService.Update(new ServiceOffering
            {
                Id = id,
                Value = model.Name,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Delete an existing Service Offering.
        /// </summary>
        /// <param name="id">Id of the model to delete.</param>
        /// <returns>The deleted model.</returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(ServiceOffering))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _serviceOfferingService.Get(id);

            // If in use, prevent update
            if (await _serviceOfferingService.IsInUse(id))
            {
                ModelState.AddModelError("ServiceOffering", $"The service offering \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _serviceOfferingService.Delete(id);

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Move an existing Service Offering.
        /// </summary>
        /// <param name="id">The Id of the model to move.</param>
        /// <param name="model">The model with updated values.</param>
        /// <returns>The moved model.</returns>
        [HttpPost("{id}/move")]
        [SwaggerResponse(200, Type = typeof(ServiceOffering))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Move(int id, ServiceOfferingModel model)
        {
            await _serviceOfferingService.Update(new ServiceOffering
            {
                Id = id,
                Value = model.Name,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);
        }
    }
}
