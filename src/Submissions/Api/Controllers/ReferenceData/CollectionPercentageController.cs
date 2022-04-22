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
    public class CollectionPercentageController : ControllerBase
    {
        private readonly IReferenceDataService<CollectionPercentage> _collectionPercentageService;
        public CollectionPercentageController(IReferenceDataService<CollectionPercentage> collectionPercentageService)
        {
            _collectionPercentageService = collectionPercentageService;
        }
        /// <summary>
        /// Generate collection percentage list
        /// </summary>
        /// <returns>A list of collection percentage</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(CollectionPercentageModel))]
        public async Task<IList> Get()
        {
            var models = (await _collectionPercentageService.List())
                .Select(x =>
                    Task.Run(async () => new CollectionPercentageModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        LowerBound = x.LowerBound,
                        UpperBound = x.UpperBound,
                    })
                    .Result
                )
                .ToList();

            return models;
        }
    }
}
