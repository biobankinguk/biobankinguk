using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Config;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory;
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
    public class DonorCountController : ControllerBase
    {
        private readonly IReferenceDataCrudService<DonorCount> _donorCountService;
        private readonly IConfigService _configService;

        public DonorCountController(
            IReferenceDataCrudService<DonorCount> donorCountService,
            IConfigService configService)
        {
            _donorCountService = donorCountService;
            _configService = configService;
        }

        /// <summary>
        /// Generate a Donor Count list.
        /// </summary>
        /// <returns>List of donor counts.</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(DonorCountModel))]
        public async Task<IList> Get()
        {
            var models = (await _donorCountService.List())
                .Select(x =>
                    Task.Run(async () => new DonorCountModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        LowerBound = x.LowerBound,
                        UpperBound = x.UpperBound,
                        SampleSetsCount = await _donorCountService.GetUsageCount(x.Id)
                    })
                    .Result
                )
                .ToList();

            return models;
        }

        /// <summary>
        /// Insert a new Donor Count
        /// </summary>
        /// <param name="model">Model to insert.</param>
        /// <returns>The inserted model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(DonorCountModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Post(DonorCountModel model)
        {
            //Getting the name of the reference type as stored in the config
            Entities.Data.Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.DonorCountName);

            // Validate model
            if (await _donorCountService.ExistsExcludingId(model.Id, model.Description))
            {
                ModelState.AddModelError("DonorCounts", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var donor = new DonorCount
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound

            };

            await _donorCountService.Add(donor);
            await _donorCountService.Update(donor);

            // Success response
            return Ok(model);
        }

        /// <summary>
        /// Update a Donor count.
        /// </summary>
        /// <param name="id">Id of the Donor Count to update.</param>
        /// <param name="model">Model of values to update with.</param>
        /// <returns>The updated model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(DonorCountModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Put(int id, DonorCountModel model)
        {
            //Getting the name of the reference type as stored in the config
            Entities.Data.Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.DonorCountName);

            // Validate model
            if (await _donorCountService.ExistsExcludingId(id, model.Description))
            {
                ModelState.AddModelError("DonorCounts", $"That {currentReferenceName.Value} already exists!");
            }

            // If in use, prevent update
            if (model.SampleSetsCount > 0)
            {
                ModelState.AddModelError("DonorCounts", $"The donor count \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _donorCountService.Update(new DonorCount
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound
            });

            // Success message
            return Ok(model);
        }

        /// <summary>
        /// Delete a Donor Count.
        /// </summary>
        /// <param name="id">Id of the Donor Count to delete.</param>
        /// <returns>The deleted Donor Count.</returns>
        /// <response code="200">Request Successful</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(DonorCountModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _donorCountService.Get(id);

            //Getting the name of the reference type as stored in the config
            Entities.Data.Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.DonorCountName);

            // If in use, prevent update
            if (await _donorCountService.IsInUse(id))
            {
                ModelState.AddModelError("DonorCounts", $"The donor count \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _donorCountService.Delete(id);

            // Success
            return Ok(model);
        }

        /// <summary>
        /// Move a Donor Count.
        /// </summary>
        /// <param name="id">Id of the Donor Count to move.</param>
        /// <param name="model">Model with updated values.</param>
        /// <returns>The moved model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPost("{id}/move")]
        [SwaggerResponse(200, Type = typeof(DonorCountModel))]
        public async Task<ActionResult> Move(int id, DonorCountModel model)
        {
            await _donorCountService.Update(new DonorCount
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound
            });

            //Everything went A-OK!
            return Ok(model);
        }
    }
}

