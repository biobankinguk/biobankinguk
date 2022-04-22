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
    public class ServiceOfferingController : ControllerBase
    {
        private readonly IReferenceDataService<ServiceOffering> _serviceOfferingService;

        public ServiceOfferingController(IReferenceDataService<ServiceOffering> serviceOfferingService)
        {
            _serviceOfferingService = serviceOfferingService;
        }
        /// <summary>
        /// Generate service offering list
        /// </summary>
        /// <returns>A list of service offering collection</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(ServiceOfferingModel))]
        public async Task<IList> Get()
        {
            var models = (await _serviceOfferingService.List())
                .Select(x =>

            Task.Run(async () => new ServiceOfferingModel
            {
                Id = x.Id,
                Name = x.Value,
                SortOrder = x.SortOrder
            }).Result)

                .ToList();

            return models;
        }
    }
}
