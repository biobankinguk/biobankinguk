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
    public class CollectionStatusController : ControllerBase
    {
        private IReferenceDataService<CollectionStatus> _collectionStatusService;

        public CollectionStatusController(IReferenceDataService<CollectionStatus> collectionStatusService)
        {
            _collectionStatusService = collectionStatusService;
        }
        /// <summary>
        /// Generate collection status list
        /// </summary>
        /// <returns>A list of collection status</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(CollectionStatusModel))]
        public async Task<IList> Get()
        {
            var model = (await _collectionStatusService.List())
                    .Select(x =>

                Task.Run(async () => new CollectionStatusModel
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
