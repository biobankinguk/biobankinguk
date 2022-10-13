using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System;
using Biobanks.Entities.Data.ReferenceData;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class ServiceOfferingController : ControllerBase
    {
        private readonly IReferenceDataService<ServiceOffering> _serviceOfferingService;

        public ServiceOfferingController(IReferenceDataService<ServiceOffering> serviceOfferingService)
        {
            _serviceOfferingService = serviceOfferingService;
        }

        [HttpGet]
        [AllowAnonymous]
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

        [HttpDelete("{id}")]
        [AllowAnonymous]
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

        [HttpPut("{id}")]
        [AllowAnonymous]
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

        [HttpPost]
        [AllowAnonymous]
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

        [HttpPost("{id}/move")]
        [AllowAnonymous]
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
