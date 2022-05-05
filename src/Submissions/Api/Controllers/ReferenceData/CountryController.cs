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
    public class CountryController : ControllerBase
    {
        private readonly IReferenceDataService<Country> _countryService;

        public CountryController(IReferenceDataService<Country> countryService)
        {
            _countryService = countryService;
        }
        /// <summary>
        /// Generate country list
        /// </summary>
        /// <returns>A list of countries</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(CountryModel))]
        public async Task<IList> Get()
        {
            var model = (await _countryService.List())
                .Select(x => new CountryModel
                {
                    Id = x.Id,
                    Name = x.Value,
                })
                .ToList();
            return model;
        }
    }
}
