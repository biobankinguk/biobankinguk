using System.Web.Http;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Services.Contracts;
using Biobanks.Services.Dto;
using Biobanks.Web.Filters;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "BiobankAdmin")]
    [RoutePrefix("api/Biobank")]
    public class BiobankController : ApiBaseController
    {
        private readonly IOrganisationService _organisationService;

        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;
        private readonly IMapper _mapper;

        public BiobankController(
            IOrganisationService organisationService,
            IBiobankReadService biobankReadService,
            IBiobankWriteService biobankWriteService,
            IMapper mapper)
        {
            _organisationService = organisationService;
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("IncludePublications/{id}")]
        public async Task<bool> IncludePublications(int id)
        {
            return await _biobankReadService.OrganisationIncludesPublications(id);
        }

        [HttpPut]
        [Route("IncludePublications/{id}/{value}")]
        public async Task IncludePublications(int id, bool value)
        {
            var organisation = await _organisationService.Get(id);

            organisation.ExcludePublications = !value;

            await _organisationService.Update(organisation);
        }
    }
}