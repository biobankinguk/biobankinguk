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
    public class ConsentRestrictionController : ControllerBase
    {
        private readonly IReferenceDataService<ConsentRestriction> _consentRestrictionService;

        public ConsentRestrictionController(IReferenceDataService<ConsentRestriction> consentRestrictionService)
        {
            _consentRestrictionService = consentRestrictionService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _consentRestrictionService.List())
                    .Select(x =>

                Task.Run(async () => new Models.Submissions.ReadConsentRestrictionModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    CollectionCount = await _consentRestrictionService.GetUsageCount(x.Id),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList();

            return model;
        }

        [HttpDelete]
        [Route("{id}")]
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

        [HttpPut]
        [Route("{id}")]
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

        [HttpPost]
        [Route("")]
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

        [HttpPost]
        [Route("{id}/move")]
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