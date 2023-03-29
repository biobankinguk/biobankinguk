using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Config;
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
    public class MacroscopicAssessmentController : ControllerBase
    {
        private readonly IReferenceDataCrudService<MacroscopicAssessment> _macroscopicAssessmentService;
        private readonly IConfigService _configService;

        public MacroscopicAssessmentController(
            IReferenceDataCrudService<MacroscopicAssessment> macroscopicAssessment,
            IConfigService configService)
        {
            _macroscopicAssessmentService = macroscopicAssessment;
            _configService = configService;
        }

        /// <summary>
        /// Generate a list of Macroscopic Assessments.
        /// </summary>
        /// <returns>List of Macroscopic Assessments.</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(MacroscopicAssessmentModel))]
        public async Task<IList> Get()
        {
            var models = (await _macroscopicAssessmentService.List())
                .Select(x =>
                    Task.Run(async () => new MacroscopicAssessmentModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _macroscopicAssessmentService.GetUsageCount(x.Id)
                    })
                    .Result
                )
                .ToList();

            return models;
        }

        /// <summary>
        /// Insert a new Macroscopic Assessment.
        /// </summary>
        /// <param name="model">Model of new Macroscopic Assessment.</param>
        /// <returns>The inserted model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(MacroscopicAssessment))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Post(MacroscopicAssessmentModel model)
        {
            //Getting the name of the reference type as stored in the config
            Entities.Data.Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

            // Validate model
            if (await _macroscopicAssessmentService.Exists(model.Description))
            {
                ModelState.AddModelError("MacroscopicAssessments", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assessment = new MacroscopicAssessment
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder
            };

            await _macroscopicAssessmentService.Add(assessment);
            await _macroscopicAssessmentService.Update(assessment);

            // Success response
            return Ok(model);
        }

        /// <summary>
        /// Update a Macroscopic Assessment.
        /// </summary>
        /// <param name="id">Id of the model to update.</param>
        /// <param name="model">Model with updates values.</param>
        /// <returns>The updated model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(MacroscopicAssessment))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Put(int id, MacroscopicAssessmentModel model)
        {
            //Getting the name of the reference type as stored in the config
            Entities.Data.Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

            // Validate model
            if (await _macroscopicAssessmentService.Exists(model.Description))
            {
                ModelState.AddModelError("MacroscopicAssessments", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (model.SampleSetsCount > 0)
            {
                ModelState.AddModelError("MacroscopicAssessments", $"This {currentReferenceName.Value} \"{model.Description}\" is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _macroscopicAssessmentService.Update(new MacroscopicAssessment
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            // Success message
            return Ok(model);
        }

        /// <summary>
        /// Delete a Macroscopic Assessment.
        /// </summary>
        /// <param name="id">Id of the model to delete.</param>
        /// <returns>The delete model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(MacroscopicAssessment))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Delete(int id)
        {

            var model = await _macroscopicAssessmentService.Get(id);

            //Getting the name of the reference type as stored in the config
            Entities.Data.Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

            if (await _macroscopicAssessmentService.IsInUse(id))
            {
                ModelState.AddModelError("MacroscopicAssessments", $"This {currentReferenceName.Value} \"{model.Value}\" is currently in use and cannot be deleted.");
            }

            if ((await _macroscopicAssessmentService.Count()) <= 1)
            {
                ModelState.AddModelError("MacroscopicAssessments", $"The {currentReferenceName.Value} \"{model.Value}\" is currently the last entry and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _macroscopicAssessmentService.Delete(id);

            // Success
            return Ok(model);
        }

        /// <summary>
        /// Move a Macroscopic Assessment.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>The updated model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPost("{id}/move")]
        [SwaggerResponse(200, Type = typeof(MacroscopicAssessment))]
        public async Task<ActionResult> Move(int id, MacroscopicAssessmentModel model)
        {
            await _macroscopicAssessmentService.Update(new MacroscopicAssessment
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

