using Directory.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Biobanks.Web.ApiControllers
{
    public class BiobankIdsController : ApiController
    {
        private readonly IBiobankReadService _biobankReadService;

        public BiobankIdsController(IBiobankReadService biobankReadService)
        {
            _biobankReadService = biobankReadService;
        }

        // GET api/BiobankIds
        public async Task<IEnumerable<string>> GetAsync()
        {
            return (await _biobankReadService.ListBiobanksAsync()).Select(x =>x.OrganisationExternalId);
        }
    }
}