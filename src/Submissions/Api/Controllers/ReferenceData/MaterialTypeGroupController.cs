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
    public class MaterialTypeGroupController : ControllerBase
    {
        private readonly IReferenceDataService<MaterialTypeGroup> _materialTypeGroupService;

        public MaterialTypeGroupController(IReferenceDataService<MaterialTypeGroup> materialTypeGroupService)
        {
            _materialTypeGroupService = materialTypeGroupService;
        }
        /// <summary>
        /// Generate material type group list
        /// </summary>
        /// <returns>A list of material type group</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(MaterialTypeGroupModel))]
        public async Task<IEnumerable> Get()
        {
            var materialTypeGroups = await _materialTypeGroupService.List();

            return materialTypeGroups.Select(x => new MaterialTypeGroupModel
            {
                Id = x.Id,
                Description = x.Value,
                MaterialTypes = x.MaterialTypes.Select(x => x.Value)
                //MaterialTypeCount = x.MaterialTypes.Count()
            });
        }
    }
}
