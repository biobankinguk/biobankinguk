using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;
using System.Collections.Generic;
using Newtonsoft.Json;
using Biobanks.Entities.Shared.ReferenceData;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/AssociatedDataType")]
    public class AssociatedDataTypeController : ApiBaseController
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

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IHttpActionResult> Get()
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

            return Json(new 
            {
                AssociatedDataTypes = model,
                AssociatedDataTypeGroups = groups
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = await _associatedDataTypeService.Get(id);

            if (await _associatedDataTypeService.IsInUse(id))
            {
                ModelState.AddModelError("AssociatedDataTypes", $"The associated data type \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _associatedDataTypeService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, AssociatedDataTypeModel model)
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
                return JsonModelInvalidResponse(ModelState);
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
            return Json(new
            {
                success = true,
                name = model.Name,
            });
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(AssociatedDataTypeModel model)
        {
            // Validate model
            if (await _associatedDataTypeService.Exists(model.Name))
            {
                ModelState.AddModelError("AssociatedDataTypes", "That name is already in use. Associated Data Type names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
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
            return Json(new
            {
                success = true,
                name = model.Name,
            });
        }
    }
}