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
    public class BiobanksController : ApiController
    {
        private readonly IBiobankReadService _biobankReadService;

        public BiobanksController(IBiobankReadService biobankReadService)
        {
            _biobankReadService = biobankReadService;
        }

        // GET api/Biobanks
        public async Task<IEnumerable<string>> GetAsync()
        {
            return (await _biobankReadService.ListBiobanksAsync()).Select(x => x.Name);
        }
    }
}