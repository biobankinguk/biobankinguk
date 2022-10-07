using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Models.Submissions;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private readonly IOntologyTermService _ontologyTermService;

        public AssociatedDataTypeController(
          IReferenceDataService<AssociatedDataType> associatedDataTypeService,
          IReferenceDataService<AssociatedDataTypeGroup> associatedDataTypeGroupService,
          IOntologyTermService ontologyTermService)
        {
            _associatedDataTypeService = associatedDataTypeService;
            _associatedDataTypeGroupService = associatedDataTypeGroupService;
            _ontologyTermService = ontologyTermService;
        }

        [HttpGet("")]
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

            return Ok(new AssociatedDataTypeWithGroup
            {
                AssociatedDataTypes = model,
                AssociatedDataTypeGroups = groups
            });
        }

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