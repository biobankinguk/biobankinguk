using System.Web.Http;
using System.Threading.Tasks;
using Biobanks.Services.Contracts;
using Biobanks.Web.Filters;
using Biobanks.Services.Dto;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "BiobankAdmin")]
    [RoutePrefix("api/Biobank")]
    public class BiobankController : ApiBaseController
    {
        private readonly IOrganisationService _organisationService;

        public BiobankController(IOrganisationService organisationService)
        {
            _organisationService = organisationService;
        }

        [HttpGet]
        [Route("IncludePublications/{id}")]
        public async Task<bool> IncludePublications(int id)
            =>  await _organisationService.UsesPublications(id);

        [HttpPut]
        [Route("IncludePublications/{id}/{value}")]
        public async Task IncludePublications(int id, bool value)
        {
            await _organisationService.Update(new OrganisationDTO
            {
                OrganisationId = id,
                ExcludePublications = !value
            });
        }
    }
}