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
    public class AssociatedDataTypeController : ControllerBase
    {
        private readonly IReferenceDataService<AssociatedDataType> _associatedDataTypeService;
        private readonly IReferenceDataService<AssociatedDataTypeGroup> _associatedDataTypeGroupService;

        public AssociatedDataTypeController(
            IReferenceDataService<AssociatedDataType> associatedDataTypeService,
            IReferenceDataService<AssociatedDataTypeGroup> associatedDataTypeGroupService)
        {
            _associatedDataTypeService = associatedDataTypeService;
            _associatedDataTypeGroupService = associatedDataTypeGroupService;
        }
        /// <summary>
        /// Generate associated data types
        /// </summary>
        /// <returns>A list of associated data types</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(AssociatedDataTypesModel))]
        public async Task<IActionResult> Get()
        {
            var associatedDataTypes = await _associatedDataTypeService.List();

            var model = associatedDataTypes
                .Select(x => new AssociatedDataTypeModel
                    {
                        Id = x.Id,
                        Name = x.Value,
                        Message = x.Message,
                        AssociatedDataTypeGroupId = x.AssociatedDataTypeGroupId,
                        AssociatedDataTypeGroupName = x.AssociatedDataTypeGroup?.Value,
                    }
               )
               .ToList();

            var groups = associatedDataTypes
                /*.Where(x => x.AssociatedDataTypeGroup != null)*/
                .GroupBy(x => x.AssociatedDataTypeGroupId)
                .Select(x => x.First())
                .Select(x => new AssociatedDataTypeGroupModel
                {
                    AssociatedDataTypeGroupId = x.Id,
                    Name = x.Value,
                })
                .ToList();

            return new JsonResult(new
            {
                AssociatedDataTypes = model,
                AssociatedDataTypeGroups = groups,
            });
        }
    }
}
