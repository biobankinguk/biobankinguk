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
    public class MaterialTypeController : ControllerBase
    {
        private readonly IReferenceDataService<MaterialType> _materialTypeService;
       
        public MaterialTypeController(
            IReferenceDataService<MaterialType> materialTypeService)
        {
            _materialTypeService = materialTypeService;
        }
        /// <summary>
        /// Generate material type list
        /// </summary>
        /// <returns>A list of material type</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(MaterialTypeModel))]
        public async Task<IList> Get()
        {
            var model = (await _materialTypeService.List())
                    .Select(x =>

                    Task.Run(async () => new MaterialTypeModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder
                    }).Result).ToList();

            return model;
        }
    }

}
