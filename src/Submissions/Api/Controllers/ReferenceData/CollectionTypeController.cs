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
    public class CollectionTypeController : ControllerBase
    {
        private readonly IReferenceDataService<CollectionType> _collectionTypeService;
        public CollectionTypeController(IReferenceDataService<CollectionType> collectionTypeService)
        {
            _collectionTypeService = collectionTypeService;
        }
        /// <summary>
        /// Generate collection type list
        /// </summary>
        /// <returns>A list of collection type</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(CollectionStatusModel))]
        public async Task<IList> Get()
        {
            var model = (await _collectionTypeService.List())
                    .Select(x => new CollectionTypeModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    SortOrder = x.SortOrder
                })
                .ToList();
            return model;
        }
    }
}
