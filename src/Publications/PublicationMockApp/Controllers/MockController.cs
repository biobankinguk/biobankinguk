using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Publications;
using Publications.Services;

namespace PublicationsMockApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MockController : ControllerBase
    {

        private IPublicationService _publicationService;

        public MockController(IPublicationService publicationService)
        {
            _publicationService = publicationService;
        }

        [HttpGet]
        public async Task<IEnumerable<PublicationDTO>> GetOrganisationPublicationsAsync(string organisationName)
        {
            return await _publicationService.GetOrganisationPublications(organisationName);
        }
    }
}
