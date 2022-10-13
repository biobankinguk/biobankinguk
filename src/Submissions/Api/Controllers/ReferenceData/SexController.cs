using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class SexController : ControllerBase
    {
        private readonly IReferenceDataService<Sex> _sexService;

        public SexController(IReferenceDataService<Sex> sexService)
        {
            _sexService = sexService;
        }

        [HttpGet]
        [AllowAnonymous]
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

        [HttpPost]
        [AllowAnonymous]
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

        [HttpPut("{id}")]
        [AllowAnonymous]
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

        [HttpDelete("{id}")]
        [AllowAnonymous]
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

        [HttpPost("{id}/move")]
        [AllowAnonymous]
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

