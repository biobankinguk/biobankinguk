using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class AssociatedDataProcurementTimeFrameController : ControllerBase
    {
        private readonly IReferenceDataService<AssociatedDataProcurementTimeframe> _associatedDataProcurementTimeFrameService;
        public AssociatedDataProcurementTimeFrameController(
        IReferenceDataService<AssociatedDataProcurementTimeframe> associatedDataProcurementTimeFrameService)
        {
            _associatedDataProcurementTimeFrameService = associatedDataProcurementTimeFrameService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IList> Get()
        {
            var models = (await _associatedDataProcurementTimeFrameService.List())
                .Select(x =>
                    Task.Run(async () => new Models.Submissions.ReadAssociatedDataProcurementTimeFrameModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        DisplayName = x.DisplayValue,
                        CollectionCapabilityCount = await _associatedDataProcurementTimeFrameService.GetUsageCount(x.Id),
                        SortOrder = x.SortOrder
                    })
                    .Result
                )
                .ToList();

            return models;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _associatedDataProcurementTimeFrameService.Get(id);

            //Validate min amount of time frames
            if (await _associatedDataProcurementTimeFrameService.Count() <= 2)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"A minimum amount of 2 time frames are allowed.");
            }

            if (await _associatedDataProcurementTimeFrameService.IsInUse(id))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"The associated data procurement time frame \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _associatedDataProcurementTimeFrameService.Delete(id);

            //Everything went A-OK!
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Post(AssociatedDataProcurementTimeFrameModel model)
        {
            // Validate model
            if (await _associatedDataProcurementTimeFrameService.Count() >= 5)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"A maximum amount of 5 time frames are allowed.");
            }

            if (await _associatedDataProcurementTimeFrameService.Exists(model.Description))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "That description is already in use. Associated Data Procurement Time Frame descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var procurement = new AssociatedDataProcurementTimeframe
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                DisplayValue = model.DisplayName
            };

            await _associatedDataProcurementTimeFrameService.Add(procurement);
            await _associatedDataProcurementTimeFrameService.Update(procurement);

            // Success response
            return Ok(model);
        }

        [HttpPost("{id}/move")]
        public async Task<IActionResult> Move(int id, AssociatedDataProcurementTimeFrameModel model)
        {
            await _associatedDataProcurementTimeFrameService.Update(new AssociatedDataProcurementTimeframe
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                DisplayValue = model.DisplayName
            });

            //Everything went A-OK!
            return Ok(model);
        }
    }
}
