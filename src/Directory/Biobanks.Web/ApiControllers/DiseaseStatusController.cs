using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Web.Filters;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Biobanks.Directory.Services.Constants;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Data.ReferenceData;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/DiseaseStatus")]
    public class DiseaseStatusController : ApiBaseController
    {
        private readonly IOntologyTermService _ontologyTermService;

        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public DiseaseStatusController(
            IOntologyTermService ontologyTermService,
            IBiobankReadService biobankReadService, 
            IBiobankWriteService biobankWriteService)
        {
            _ontologyTermService = ontologyTermService;
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
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

        [HttpGet]
        [AllowAnonymous]
        [Route("{id}/AssociatedDataTypes")]
        public async Task<List<AssociatedDataType>> GetAssociatedDataTypes(string id)
        {
           
            var associatedDataTypes = await _ontologyTermService.ListAssociatedDataTypesByOntologyTerm(id);
            return associatedDataTypes;
            
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            var model = await _ontologyTermService.Get(id, tags: new List<string> { SnomedTags.Disease });

            if (await _ontologyTermService.IsInUse(id))
            {
                ModelState.AddModelError("Description", $"The disease status \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _ontologyTermService.Delete(model.Id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(string id, OntologyTermModel model)
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
                return JsonModelInvalidResponse(ModelState);
            }
            var associatedData = ((List<AssociatedDataTypeModel>)JsonConvert.DeserializeObject(model.AssociatedDataTypesJson, typeof(List<AssociatedDataTypeModel>)));
            List<AssociatedDataType> types = await _ontologyTermService.GetAssociatedDataFromList(associatedData.Select(x => x.Id).ToList());
            await _ontologyTermService.Update(new OntologyTerm
            {
                Id = id,
                Value = model.Description,
                OtherTerms = model.OtherTerms,
                SnomedTagId = (await _biobankReadService.GetSnomedTagByDescription("Disease")).Id,
                DisplayOnDirectory = model.DisplayOnDirectory,
                AssociatedDataTypes = types
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(OntologyTermModel model)
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

            var context = new ValidationContext( model,serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(model,context, validationResults, true);
            if (!isValid)
            {
                foreach(var item in validationResults)
                {
                    ModelState.AddModelError(item.ToString(),item.ErrorMessage);
                }
            }



            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }
            var associatedData = ((List<AssociatedDataTypeModel>)JsonConvert.DeserializeObject(model.AssociatedDataTypesJson, typeof(List<AssociatedDataTypeModel>)));
            List<AssociatedDataType> types = await _ontologyTermService.GetAssociatedDataFromList(associatedData.Select(x => x.Id).ToList());

            await _ontologyTermService.Create(new OntologyTerm
            {
                Id = model.OntologyTermId,
                Value = model.Description,
                OtherTerms = model.OtherTerms,
                SnomedTagId = (await _biobankReadService.GetSnomedTagByDescription("Disease")).Id,
                DisplayOnDirectory = model.DisplayOnDirectory,
                AssociatedDataTypes = types
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }
    }
}