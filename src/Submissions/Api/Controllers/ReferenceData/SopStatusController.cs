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
    public class SopStatusController : ControllerBase
    {
        private readonly IReferenceDataService<SopStatus> _sopStatusService;

        public SopStatusController(IReferenceDataService<SopStatus> sopStatusService)
        {
            _sopStatusService = sopStatusService;
        }
        /// <summary>
        /// Generate Sop status list
        /// </summary>
        /// <returns>A list of Sop status</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(SopStatusModel))]
        public async Task<IList> Get()
        {
            var models = (await _sopStatusService.List())
                .Select(x =>
                    Task.Run(async () => new SopStatusModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                    })
                    .Result
                )
                .ToList();
            return models;
        }
    }
}
