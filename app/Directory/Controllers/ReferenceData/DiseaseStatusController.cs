using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.Directory.Controllers.ReferenceData
{
    [Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
    [Route("api/[controller]")]
    [ApiController]
    public class DiseaseStatusController : ControllerBase
    {
        private readonly IOntologyTermService _ontologyTermService;
        private readonly IDiseaseStatusService _diseaseStatusService;

        public DiseaseStatusController(
            IOntologyTermService ontologyTermService,
            IDiseaseStatusService diseaseStatusService)
        {
            _ontologyTermService = ontologyTermService;
            _diseaseStatusService = diseaseStatusService;
        }

        /// <summary>
        /// Generate a list of Ontology Term.
        /// </summary>
        /// <returns>The list of Ontology Term.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IList> Get()
        {
            var diseaseTerms = await _ontologyTermService.List(tags: new List<string>
            {
                SnomedTags.Disease
            });

            return diseaseTerms.Select(x =>

                Task.Run(async () => new ReadOntologyTermModel
                {
                    OntologyTermId = x.Id,
                    Description = x.Value,
                    CollectionCapabilityCount = await _ontologyTermService.CountCollectionCapabilityUsage(x.Id),
                    OtherTerms = x.OtherTerms,
                    DisplayOnDirectory = x.DisplayOnDirectory,
                    AssociatedDataTypes = x.AssociatedDataTypes.Select(y => new AssociatedDataTypeModel
                    {
                        Id = y.Id,
                        Name = y.Value
                    }).ToList()
                })
                .Result
            )
            .ToList();
        }

        /// <summary>
        /// Generate a list of Associated Data Types.
        /// </summary>
        /// <returns>Associated Data Types.</returns>
        [HttpGet("{id}/AssociatedDataTypes")]
        [AllowAnonymous]
        public async Task<List<AssociatedDataType>> GetAssociatedDataTypes(string id)
        {

            var associatedDataTypes = await _ontologyTermService.ListAssociatedDataTypesByOntologyTerm(id);
            return associatedDataTypes;

        }

        /// <summary>
        /// Delete a given OntologyTerm with Id
        /// </summary>
        /// <param name="id">Id of the OntologyTerm to delete.</param>
        /// <returns>The deleted OntologyTerm.</returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(OntologyTermModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Delete(string id)
        {
            var model = await _ontologyTermService.Get(id, tags: new List<string> { SnomedTags.Disease });

            if (await _ontologyTermService.IsInUse(id))
            {
                ModelState.AddModelError("Description", $"The disease status \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _ontologyTermService.Delete(model.Id);

            //Everything went A-OK!
            return Ok(model);

        }

        /// <summary>
        /// Update an exisiting OntologyTerm with the provided updated entity.
        /// </summary>
        /// <param name="id">Id of the model to update.</param>
        /// <param name="model">Model with updated values.</param>
        /// <returns>The updated OntologyTerm.</returns>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(OntologyTermModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Put(string id, OntologyTermModel model)
        {
            //If this description is valid, it already exists
            if (await _ontologyTermService.Exists(id, value: model.Description, filterById: false))
            {
                ModelState.AddModelError("Description", "That description is already in use. Descriptions must be unique across all ontology terms.");
            }

            if (await _ontologyTermService.IsInUse(id))
            {
                //Allow editing of only Other terms field if diagnosis in use
                var diagnosis = await _ontologyTermService.Get(id, tags: new List<string> { SnomedTags.Disease });

                if (diagnosis.Value != model.Description)
                    ModelState.AddModelError("Description", "This disease status is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var associatedData = (List<AssociatedDataTypeModel>)JsonConvert.DeserializeObject(model.AssociatedDataTypesJson, typeof(List<AssociatedDataTypeModel>));
            List<AssociatedDataType> types = await _ontologyTermService.GetAssociatedDataFromList(associatedData.Select(x => x.Id).ToList());
            await _ontologyTermService.Update(new OntologyTerm
            {
                Id = id,
                Value = model.Description,
                OtherTerms = model.OtherTerms,
                SnomedTagId = (await _diseaseStatusService.GetSnomedTagByDescription("Disease")).Id,
                DisplayOnDirectory = model.DisplayOnDirectory,
                AssociatedDataTypes = types
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Create a new OntologyTerm. The Id of the OntologyTerm should be null, as it is
        /// generated by the database
        /// </summary>
        /// <param name="model">The model to insert.</param>
        /// <returns>The newly created OntologyTerm, with assigned Id</returns>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(OntologyTermModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IActionResult> Post(OntologyTermModel model)
        {
            // Had to do this as it is not binding to ontology term model
            ModelState.Clear();
            //if ontology term id is in use by another ontology term
            if (await _ontologyTermService.Exists(id: model.OntologyTermId))
                ModelState.AddModelError("OntologyTermId", "That ID is already in use. IDs must be unique across all ontology terms.");

            //If this description is valid, it already exists
            if (await _ontologyTermService.Exists(value: model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Descriptions must be unique across all ontology terms.");
            }

            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(model, context, validationResults, true);
            if (!isValid)
            {
                foreach (var item in validationResults)
                {
                    ModelState.AddModelError(item.ToString(), item.ErrorMessage);
                }
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var associatedData = (List<AssociatedDataTypeModel>)JsonConvert.DeserializeObject(model.AssociatedDataTypesJson, typeof(List<AssociatedDataTypeModel>));
            List<AssociatedDataType> types = await _ontologyTermService.GetAssociatedDataFromList(associatedData.Select(x => x.Id).ToList());
            await _ontologyTermService.Create(new OntologyTerm
            {
                Id = model.OntologyTermId,
                Value = model.Description,
                OtherTerms = model.OtherTerms,
                SnomedTagId = (await _diseaseStatusService.GetSnomedTagByDescription("Disease")).Id,
                DisplayOnDirectory = model.DisplayOnDirectory,
                AssociatedDataTypes = types
            });


            //Everything went A-OK!
            return Ok(model);
        }
    }
}
