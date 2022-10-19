﻿using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Models.Submissions;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    //TODO: Register IOrganisationService in startup.cs

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
        [HttpGet("IncludePublications/{id}")]
        public async Task<bool> IncludePublications(int id)
            => await _organisationService.UsesPublications(id);

        /// <summary>
        /// Update an exisiting Organisation
        /// </summary>
        [HttpPut("IncludePublications/{id}/{value}")]
        public async Task IncludePublications(int id, bool value)
        {
            var organisation = await _organisationService.Get(id);
            organisation.ExcludePublications = !value;
            await _organisationService.Update(organisation);
        }
    }
}