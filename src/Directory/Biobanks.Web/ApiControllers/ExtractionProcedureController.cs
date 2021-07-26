using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Web.Filters;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/ExtractionProcedure")]
    public class ExtractionProcedureController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public ExtractionProcedureController(IBiobankReadService biobankReadService, IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            return (await _biobankReadService.ListExtractionProceduresAsync())
                .Select(x =>

                Task.Run(async () => new ReadExtractionProcedureModel
                {
                    OntologyTermId = x.Id,
                    Description = x.Value,
                    MaterialDetailsCount = await _biobankReadService.GetExtractionProcedureMaterialDetailsCount(x.Id),
                    OtherTerms = x.OtherTerms,
                    MaterialTypeIds = x.MaterialTypes.Select(x => x.Id).ToList()
                })
                .Result
            )
            .ToList();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            var model = await _biobankReadService.GetExtractionProcedureById(id);

            if (await _biobankReadService.IsExtractionProcedureInUse(id))
            {
                ModelState.AddModelError("Description", $"The extraction procedure \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteOntologyTermAsync(new OntologyTerm
            {
                Id = model.Id,
                Value = model.Value
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(string id, ReadExtractionProcedureModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidOntologyTerm(id, description: model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Descriptions must be unique across all ontology terms.");
            }

            //Extraction procedure should belong to at least one material type 
            if (model.MaterialTypeIds == null || model.MaterialTypeIds.Count == 0)
                ModelState.AddModelError("MaterialTypeIds", "Add at least one material type to the extraction procedure.");

            var ontologyTerm = await _biobankReadService.GetExtractionProcedureById(id);
            if (await _biobankReadService.IsExtractionProcedureInUse(id))
            {
                //Allow editing of only Other terms field if ontologyterm in use
                if (ontologyTerm.Value != model.Description)
                    ModelState.AddModelError("Description", "This extraction procedure is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateOntologyTermWithMaterialTypesAsync(new OntologyTerm
            {
                Id = id,
                Value = model.Description,
                OtherTerms = model.OtherTerms,
                SnomedTagId = ontologyTerm.SnomedTagId,
                DisplayOnDirectory = true
            },model.MaterialTypeIds);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(ReadExtractionProcedureModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidOntologyTerm(description: model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Descriptions must be unique across all ontology terms.");
            }

            //if ontology term id is in use by another ontology term
            if ((await _biobankReadService.ListExtractionProceduresAsync()).Any(x => x.Id == model.OntologyTermId))
                ModelState.AddModelError("OntologyTermId", "That ID is already in use. IDs must be unique across all ontology terms.");

            //Extraction procedure should belong to at least one material type 
            if (model.MaterialTypeIds == null || model.MaterialTypeIds.Count == 0)
                ModelState.AddModelError("MaterialTypeIds", "Add at least one material type to the extraction procedure.");

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddOntologyTermWithMaterialTypesAsync(new OntologyTerm
            {
                Id = model.OntologyTermId,
                Value = model.Description,
                OtherTerms = model.OtherTerms,
                SnomedTagId = (await _biobankReadService.GetSnomedTagByDescription("Extraction Procedure")).Id,
                DisplayOnDirectory = true
            }, model.MaterialTypeIds);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }
    }
}