using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Auth;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class AssociatedDataTypeController : ControllerBase
    {
        private readonly IReferenceDataCrudService<AssociatedDataType> _associatedDataTypeService;
        private readonly IReferenceDataCrudService<AssociatedDataTypeGroup> _associatedDataTypeGroupService;
        private readonly IOntologyTermService _ontologyTermService;
        public AssociatedDataTypeController(
            IReferenceDataCrudService<AssociatedDataType> associatedDataTypeService,
            IReferenceDataCrudService<AssociatedDataTypeGroup> associatedDataTypeGroupService,
            IOntologyTermService ontologyTermService)
        {
            _associatedDataTypeService = associatedDataTypeService;
            _associatedDataTypeGroupService = associatedDataTypeGroupService;
            _ontologyTermService = ontologyTermService;
        }

        /// <summary>
        /// Generate a list of Associated Data Type.
        /// </summary>
        /// <returns>List of Associated Data Type.</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var associatedDataTypes = await _associatedDataTypeService.List();

            var model = associatedDataTypes
                .Select(x =>
                    Task.Run(async () => new AssociatedDataTypeModel
                    {
                        Id = x.Id,
                        Name = x.Value,
                        Message = x.Message,
                        CollectionCapabilityCount = await _associatedDataTypeService.GetUsageCount(x.Id),
                        AssociatedDataTypeGroupId = x.AssociatedDataTypeGroupId,
                        AssociatedDataTypeGroupName = x.AssociatedDataTypeGroup?.Value,
                    })
                    .Result
               )
               .ToList();

            var groups = associatedDataTypes
                .Where(x => x.AssociatedDataTypeGroup != null)
                .GroupBy(x => x.AssociatedDataTypeGroupId)
                .Select(x => x.First())
                .Select(x => new AssociatedDataTypeGroupModel
                {
                    AssociatedDataTypeGroupId = x.Id,
                    Name = x.Value,
                })
                .ToList();

            return Ok(model);

        }

        /// <summary>
        /// Delete an Associated Data Type.
        /// </summary>
        /// <param name="id">Id of the model to delete.</param>
        /// <returns>The delete model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _associatedDataTypeService.Get(id);

            if (await _associatedDataTypeService.IsInUse(id))
            {
                ModelState.AddModelError("AssociatedDataTypes", $"The associated data type \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _associatedDataTypeService.Delete(id);

            //Everything went A-OK!
            return Ok(model);

        }

        /// <summary>
        /// Update an Associated Data Type.
        /// </summary>
        /// <param name="id">Id of the model to update.</param>
        /// <param name="model">Model with updates values.</param>
        /// <returns>The updated model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, AssociatedDataTypeModel model)
        {
            // Validate model
            if (await _associatedDataTypeService.ExistsExcludingId(id, model.Name))
            {
                ModelState.AddModelError("AssociatedDataTypes", "That associated data type already exists!");
            }

            if (await _associatedDataTypeService.IsInUse(id))
            {
                ModelState.AddModelError("AssociatedDataTypes", "This associated data type is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ontologyTerms = ((List<OntologyTermModel>)JsonConvert.DeserializeObject(model.OntologyTermsJson, typeof(List<OntologyTermModel>)));
            List<OntologyTerm> terms = await _ontologyTermService.GetOntologyTermsFromList(ontologyTerms.Select(x => x.OntologyTermId).ToList());
            await _associatedDataTypeService.Update(new AssociatedDataType
            {
                Id = id,
                AssociatedDataTypeGroupId = model.AssociatedDataTypeGroupId,
                Value = model.Name,
                Message = model.Message,
                OntologyTerms = terms
            });

            // Success response
            return Ok(model);

        }

        /// <summary>
        /// Insert a new Associated Data Type.
        /// </summary>
        /// <param name="model">Model of new  Associated Data Type.</param>
        /// <returns>The inserted model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPost("")]
        public async Task<IActionResult> Post(AssociatedDataTypeModel model)
        {
            // Validate model
            if (await _associatedDataTypeService.Exists(model.Name))
            {
                ModelState.AddModelError("AssociatedDataTypes", "That name is already in use. Associated Data Type names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ontologyTerms = ((List<OntologyTermModel>)JsonConvert.DeserializeObject(model.OntologyTermsJson, typeof(List<OntologyTermModel>)));
            List<OntologyTerm> terms = await _ontologyTermService.GetOntologyTermsFromList(ontologyTerms.Select(x => x.OntologyTermId).ToList());
            var associatedDataType = new AssociatedDataType
            {
                Id = model.Id,
                AssociatedDataTypeGroupId = model.AssociatedDataTypeGroupId,
                Value = model.Name,
                Message = model.Message,
                OntologyTerms = terms
            };

            await _associatedDataTypeService.Add(associatedDataType);

            // Success response
            return Ok(model);

        }
    }
}
