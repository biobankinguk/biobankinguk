using Biobanks.Entities.Shared.ReferenceData;
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
    public class SexController : ControllerBase
    {
        private readonly IReferenceDataService<Sex> _sexService;

        public SexController(IReferenceDataService<Sex> sexService)
        {
            _sexService = sexService;
        }
        /// <summary>
        /// Generate sex list
        /// </summary>
        /// <returns>A list of sex</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(SexModel))]
        public async Task<IList> Get()
        {
            var model = (await _sexService.List())
                .Select(x =>
                    Task.Run(async () => new SexModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder
                    }).Result)
                .ToList();

            return model;
        }
    }
}
