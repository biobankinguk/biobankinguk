using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Web.Filters;
using System.Collections.Generic;
using Biobanks.Directory.Services.Constants;
using Biobanks.Directory.Services.Contracts;
using DataAnnotationsExtensions;

namespace Biobanks.Web.ApiControllers
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/ExtractionProcedure")]
    public class ExtractionProcedureController : ApiBaseController
    {
        private readonly IReferenceDataService<MaterialType> _materialTypeService;

        private readonly IOntologyTermService _ontologyTermService;

        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public ExtractionProcedureController(
            IReferenceDataService<MaterialType> materialTypeService,
            IOntologyTermService ontologyTermService,
            IBiobankReadService biobankReadService, 
            IBiobankWriteService biobankWriteService)
        {
            _materialTypeService = materialTypeService;
            _ontologyTermService = ontologyTermService;
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
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
                    MaterialDetailsCount = await _biobankReadService.GetExtractionProcedureMaterialDetailsCount(x.Id),
                    OtherTerms = x.OtherTerms,
                    MaterialTypeIds = x.MaterialTypes.Select(x => x.Id).ToList(),
                    DisplayOnDirectory = x.DisplayOnDirectory
                })
                .Result
            )
            .ToList();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            var model = await _ontologyTermService.Get(id, tags: new List<string>
            {
                SnomedTags.ExtractionProcedure
            });

            if (await _biobankReadService.IsExtractionProcedureInUse(id))
            {
                ModelState.AddModelError("Description", $"The extraction procedure \"{model.Value}\" is currently in use, and cannot be deleted.");
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
        public async Task<IHttpActionResult> Put(string id, ReadExtractionProcedureModel model)
        {
            //If this description is valid, it already exists
            //if ((await _ontologyTermService.List()).Any(x=>x.Value == model.Description && x.Id != id))
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
                return JsonModelInvalidResponse(ModelState);
            }

            var materialTypes = await _materialTypeService.List();

            await _ontologyTermService.Create(new OntologyTerm
            {
                Id = model.OntologyTermId,
                Value = model.Description,
                OtherTerms = model.OtherTerms,
                SnomedTagId = (await _biobankReadService.GetSnomedTagByDescription(SnomedTags.ExtractionProcedure)).Id,
                DisplayOnDirectory = model.DisplayOnDirectory,
                MaterialTypes = materialTypes.Where(x => model.MaterialTypeIds.Contains(x.Id)).ToList()
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