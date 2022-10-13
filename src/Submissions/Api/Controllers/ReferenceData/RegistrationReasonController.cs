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

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class RegistrationReasonController : ControllerBase
    {
        private readonly IReferenceDataService<RegistrationReason> _registrationReasonService;

        public RegistrationReasonController(IReferenceDataService<RegistrationReason> registrationReasonService)
        {
            _registrationReasonService = registrationReasonService;
        }

        [HttpGet]
        [AllowAnonymous]
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

        [HttpPost]
        [AllowAnonymous]
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

        [HttpPut("{id}")]
        [AllowAnonymous]
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

        [HttpDelete("{id}")]
        [AllowAnonymous]
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
