using System.Web.Http;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Services.Contracts;
using Biobanks.Services.Dto;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/Biobank")]
    public class BiobankController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;
        private readonly IMapper _mapper;

        public BiobankController(IBiobankReadService biobankReadService,
                                 IBiobankWriteService biobankWriteService,
                                 IMapper mapper)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
            _mapper = mapper;
        }

        // GET: Biobank
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
            var biobank = _mapper.Map<OrganisationDTO>(await _biobankReadService.GetBiobankByIdAsync(id));
            biobank.ExcludePublications = !(value);
            await _biobankWriteService.UpdateBiobankAsync(biobank);
        }
    }
}