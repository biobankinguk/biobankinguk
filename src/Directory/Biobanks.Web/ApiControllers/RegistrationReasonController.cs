﻿using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using RegistrationReasonModel = Biobanks.Web.Models.Shared.RegistrationReasonModel;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/RegistrationReason")]
    public class RegistrationReasonController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public RegistrationReasonController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }


        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
                var model = (await _biobankReadService.ListRegistrationReasonsAsync())
                    .Select(x =>

                Task.Run(async () => new ReadRegistrationReasonModel
                {
                    Id = x.RegistrationReasonId,
                    Description = x.Description,
                    OrganisationCount = await _biobankReadService.GetRegistrationReasonOrganisationCount(x.RegistrationReasonId),
                }).Result)

                    .ToList();

            return model;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListRegistrationReasonsAsync()).Where(x => x.RegistrationReasonId == id).First();

            if (await _biobankReadService.IsRegistrationReasonInUse(id))
            {
                ModelState.AddModelError("Description", $"The registration reason \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteRegistrationReasonAsync(new RegistrationReason
            {
                RegistrationReasonId = model.RegistrationReasonId,
                Description = model.Description
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, RegistrationReasonModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidRegistrationReasonDescriptionAsync(id, model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use by another registration reason. Registration reason descriptions must be unique.");
            }

            if (await _biobankReadService.IsRegistrationReasonInUse(id))
            {
                ModelState.AddModelError("Description", "This registration reason is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateRegistrationReasonAsync(new RegistrationReason
            {
                RegistrationReasonId = id,
                Description = model.Description
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
        public async Task<IHttpActionResult> Post(RegistrationReasonModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidRegistrationReasonDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Registration reason descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddRegistrationReasonAsync(new RegistrationReason
            {
                Description = model.Description
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