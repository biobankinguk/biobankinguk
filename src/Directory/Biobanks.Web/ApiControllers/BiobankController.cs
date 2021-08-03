using System.Web.Http;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Services.Contracts;
using Biobanks.Services.Dto;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;

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
            var biobank = _mapper.Map<OrganisationDTO>(await _organisationService.GetBiobankByIdAsync(id));
            biobank.ExcludePublications = !(value);
            await _organisationService.UpdateBiobankAsync(biobank);
        }
    }
}