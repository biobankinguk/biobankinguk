﻿using Biobanks.Services.Contracts;
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
    [RoutePrefix("api/DiseaseStatus")]
    public class DiseaseStatusController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public DiseaseStatusController(IBiobankReadService biobankReadService, IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            return (await _biobankReadService.ListOntologyTermsAsync()).Select(x =>

                Task.Run(async () => new ReadOntologyTermModel
                {
                    OntologyTermId = x.Id,
                    Description = x.Value,
                    CollectionCapabilityCount = await _biobankReadService.GetOntologyTermCollectionCapabilityCount(x.Id),
                    OtherTerms = x.OtherTerms
                })
                .Result
            )
            .ToList();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            var model = (await _biobankReadService.ListOntologyTermsAsync()).Where(x => x.Id == id).First();

            if (await _biobankReadService.IsOntologyTermInUse(id))
            {
                ModelState.AddModelError("Description", $"The disease status \"{model.Value}\" is currently in use, and cannot be deleted.");
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
        public async Task<IHttpActionResult> Put(string id, OntologyTermModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidOntologyTermDescriptionAsync(id, model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use by another disease status. Disease status descriptions must be unique.");
            }

            if (await _biobankReadService.IsOntologyTermInUse(id))
            {
                //Allow editing of only Other terms field if diagnosis in use
                var diagnosis = (await _biobankReadService.ListOntologyTermsAsync()).Where(x => x.Id == id).First();
                if ((diagnosis.Value != model.Description) || (diagnosis.Value != model.Description))
                    ModelState.AddModelError("Description", "This disease status is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateOntologyTermAsync(new OntologyTerm
            {
                Id = model.OntologyTermId,
                Value = model.Description,
                OtherTerms = model.OtherTerms,
                DisplayOnDirectory = true
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
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidOntologyTermDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Disease status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddOntologyTermAsync(new OntologyTerm
            {
                Id = model.OntologyTermId,
                Value = model.Description,
                OtherTerms = model.OtherTerms,
                DisplayOnDirectory = true
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