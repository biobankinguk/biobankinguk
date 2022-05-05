using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Shared.Services.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class CountyController : ControllerBase
    {
        private readonly IReferenceDataService<County> _countyService;
        private readonly IReferenceDataService<Country> _countryService;

        public CountyController(
            IReferenceDataService<County> countyService,
            IReferenceDataService<Country> countryService)
        {
            _countyService = countyService;
            _countryService = countryService;
        }
        /// <summary>
        /// Generate county list
        /// </summary>
        /// <returns>A list of counties</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(CountyModel))]
        public async Task<CountiesModel> Get()
        {
            var countries = await _countryService.List();

            var model = new CountiesModel
            {
                Counties = countries.ToDictionary(
                        x => x.Value,
                        x => x.Counties.Select(county =>                           
                                new CountyModel
                                {
                                    Id = county.Id,
                                    CountryId = x.Id,
                                    Name = county.Value
                                }
                                )
                        )
            };
               return model;
        }
    }
}
