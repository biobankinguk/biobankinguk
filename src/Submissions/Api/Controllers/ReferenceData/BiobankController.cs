using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Models.Submissions;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class BiobankController : ControllerBase
    {
        private readonly IOrganisationDirectoryService _organisationService;

        public BiobankController(IOrganisationDirectoryService organisationService)
        {
            _organisationService = organisationService;
        }

        /// <summary>
        /// If this Organisation make use of the Publication feature. Then, the Directory will attempt to source
        /// relevant publications associated with this Organisation
        /// </summary>
        /// <returns> The Organisation Reference </returns>
        [HttpGet("IncludePublications/{id}")]
        [SwaggerResponse(200, Type = typeof(Organisation))]

        public async Task<bool> IncludePublications(int id)
        {
            var model = await _organisationService.UsesPublications(id);
            return model;
        }


        /// <summary>
        /// Update an exisiting Organisation
        /// </summary>
        /// <returns>The updated Organisation reference</returns>
        [HttpPut("IncludePublications/{id}/{value}")]
        [SwaggerResponse(200, Type = typeof(Organisation))]

        public async Task<IActionResult> IncludePublications(int id, bool value)
        {
            var organisation = await _organisationService.Get(id);
            organisation.ExcludePublications = !value;
            var model = await _organisationService.Update(organisation);

            return Ok(model);
        }
    }
}