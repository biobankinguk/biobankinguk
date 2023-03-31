using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Data.Entities.Shared.ReferenceData;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Models.Shared;
using Biobanks.Directory.Services.Directory.Constants;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.Directory.Controllers.ReferenceData
{
    [Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class ExtractionProcedureController : ControllerBase
    {
        private readonly IMaterialTypeService _materialTypeService;

        private readonly IOntologyTermService _ontologyTermService;

        public ExtractionProcedureController(
            IMaterialTypeService materialTypeService,
            IOntologyTermService ontologyTermService)
        {
            _materialTypeService = materialTypeService;
            _ontologyTermService = ontologyTermService;
        }

        /// <summary>
        /// Generate a list of Extraction Procedures.
        /// </summary>
        /// <returns>List of Extraction Procedures.</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(MacroscopicAssessment))]
        public async Task<IList> Get()
        {
            return (await _ontologyTermService.List(tags: new List<string>
                {
                    SnomedTags.ExtractionProcedure
                }))
                .Select(x =>

                Task.Run(async () => new ReadExtractionProcedureModel
                {
                    OntologyTermId = x.Id,
                    Description = x.Value,
                    MaterialDetailsCount = await _materialTypeService.GetExtractionProcedureMaterialDetailsCount(x.Id),
                    OtherTerms = x.OtherTerms,
                    MaterialTypeIds = x.MaterialTypes.Select(x => x.Id).ToList(),
                    DisplayOnDirectory = x.DisplayOnDirectory
                })
                .Result
            )
            .ToList();
        }

        /// <summary>
        /// Insert a new Extraction Procedure.
        /// </summary>
        /// <param name="model">The model to insert.</param>
        /// <returns>The inserted model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPost]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(ReadExtractionProcedureModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Post(ReadExtractionProcedureModel model)
        {
            //If this description is valid, it already exists
            if (await _ontologyTermService.Exists(value: model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Descriptions must be unique across all ontology terms.");
            }

            //if ontology term id is in use by another ontology term
            if (await _ontologyTermService.Exists(id: model.OntologyTermId))
                ModelState.AddModelError("OntologyTermId", "That ID is already in use. IDs must be unique across all ontology terms.");

            //Extraction procedure should belong to at least one material type 
            if (model.MaterialTypeIds == null || model.MaterialTypeIds.Count == 0)
                ModelState.AddModelError("MaterialTypeIds", "Add at least one material type to the extraction procedure.");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var materialTypes = await _materialTypeService.List();

            await _ontologyTermService.Create(new OntologyTerm
            {
                Id = model.OntologyTermId,
                Value = model.Description,
                OtherTerms = model.OtherTerms,
                SnomedTagId = (await _ontologyTermService.GetSnomedTagByDescription(SnomedTags.ExtractionProcedure)).Id,
                DisplayOnDirectory = model.DisplayOnDirectory,
                MaterialTypes = materialTypes.Where(x => model.MaterialTypeIds.Contains(x.Id)).ToList()
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Update a Extraction Procedure.
        /// </summary>
        /// <param name="id">Id of the Extraction Procedure to update.</param>
        /// <param name="model">The model with updated values.</param>
        /// <returns>The updated model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPut("{id}")]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(OntologyTerm))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Put(string id, ReadExtractionProcedureModel model)
        {
            //If this description is valid, it already exists
            var ontologyTerm = await _ontologyTermService.Get(value: model.Description, tags: new List<string>
            {
                SnomedTags.ExtractionProcedure
            });

            if (ontologyTerm != null && ontologyTerm.Id != id)
            {
                ModelState.AddModelError("Description", "That description is already in use. Descriptions must be unique across all ontology terms.");
            }
            else if (ontologyTerm is null)
            {
                ontologyTerm = await _ontologyTermService.Get(id);
            }

            //Extraction procedure should belong to at least one material type 
            if (model.MaterialTypeIds == null || model.MaterialTypeIds.Count == 0)
                ModelState.AddModelError("MaterialTypeIds", "Add at least one material type to the extraction procedure.");

            // Check if its in use.
            if (await _materialTypeService.GetExtractionProcedureMaterialDetailsCount(id) > 0)
            {
                //Allow editing of only Other terms field if ontologyterm in use
                if (ontologyTerm.Value != model.Description)
                    ModelState.AddModelError("Description", "This extraction procedure is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _ontologyTermService.Update(new OntologyTerm
            {
                Id = id,
                Value = model.Description,
                OtherTerms = model.OtherTerms,
                SnomedTagId = ontologyTerm.SnomedTagId,
                DisplayOnDirectory = model.DisplayOnDirectory,
                MaterialTypes = (await _materialTypeService.List()).Where(x => model.MaterialTypeIds.Contains(x.Id)).ToList()
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Delete an Extraction Procedure.
        /// </summary>
        /// <param name="id">Id of the model to delete.</param>
        /// <returns>The delete model.</returns>
        /// <response code="200">Request Successful</response>
        [HttpDelete("{id}")]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(OntologyTerm))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Delete(string id)
        {
            var model = await _ontologyTermService.Get(id, tags: new List<string>
            {
                SnomedTags.ExtractionProcedure
            });

            // Check if its in use.
            if (await _materialTypeService.GetExtractionProcedureMaterialDetailsCount(id) > 0)
            {
                ModelState.AddModelError("Description", $"The extraction procedure \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _ontologyTermService.Delete(model.Id);

            //Everything went A-OK!
            return Ok(id);
        }
    }
}
