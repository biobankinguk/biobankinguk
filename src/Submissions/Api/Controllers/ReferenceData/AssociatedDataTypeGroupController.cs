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
    public class AssociatedDataTypeGroupController : ControllerBase
    {
        private readonly IReferenceDataService<AssociatedDataTypeGroup> _associatedDataTypeGroupService;

        public AssociatedDataTypeGroupController(IReferenceDataService<AssociatedDataTypeGroup> associatedDataTypeGroupService)
        {
            _associatedDataTypeGroupService = associatedDataTypeGroupService;
        }
        /// <summary>
        /// Generate associated data types group list
        /// </summary>
        /// <returns>A list of associated data types group</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(AssociatedDataTypeGroupModel))]
        public async Task<IList> Get()
        {
            var model = (await _associatedDataTypeGroupService.List())
                .Select(x =>
                    Task.Run(async () => new AssociatedDataTypeGroupModel
                    {
                        AssociatedDataTypeGroupId = x.Id,
                        Name = x.Value,
                    })
                    .Result
                 )
                .ToList();

            return model;
        }
    }
}
