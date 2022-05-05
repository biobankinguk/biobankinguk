using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Shared.Services.Contracts;
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
    public class MacroscopicAssessmentController : ControllerBase
    {
        private readonly IReferenceDataService<MacroscopicAssessment> _macroscopicAssessmentService;
        public MacroscopicAssessmentController(
            IReferenceDataService<MacroscopicAssessment> macroscopicAssessment)
        {
            _macroscopicAssessmentService = macroscopicAssessment;
        }
        /// <summary>
        /// Generate macroscopic assessment list
        /// </summary>
        /// <returns>A list of macroscopic assessment</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(MacroscopicAssessmentModel))]
        public async Task<IList> Get()
        {
            var models = (await _macroscopicAssessmentService.List())
                .Select(x => new MacroscopicAssessmentModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder
                    }
                )
                .ToList();
            return models;
        }
    }
}
