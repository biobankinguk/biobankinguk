using System.Linq;
 using System;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using RegistrationReasonModel = Biobanks.Web.Models.Shared.RegistrationReasonModel;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/RegistrationReason")]
    public class RegistrationReasonController : ApiBaseController
    {
        private readonly IReferenceDataService<RegistrationReason> _registrationReasonService;

        public RegistrationReasonController(IReferenceDataService<RegistrationReason> registrationReasonService)
        {
            _registrationReasonService = registrationReasonService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
                var model = (await _registrationReasonService.List())
                    .Select(x =>

                Task.Run(async () => new ReadRegistrationReasonModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    OrganisationCount = await _registrationReasonService.GetUsageCount(x.Id),
                }).Result)

                    .ToList();

            return model;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = await _registrationReasonService.Get(id);

            if (await _registrationReasonService.IsInUse(id))
            {
                ModelState.AddModelError("Description", $"The registration reason \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _registrationReasonService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, RegistrationReasonModel model)
        {
            var existing = await _registrationReasonService.Get(model.Description);

            //If this description is valid, it already exists
            if (existing != null && existing.Id != id)
            {
                ModelState.AddModelError("Description", "That description is already in use by another registration reason. Registration reason descriptions must be unique.");
            }

            if (await _registrationReasonService.IsInUse(id))
            {
                ModelState.AddModelError("Description", "This registration reason is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _registrationReasonService.Update(new RegistrationReason
            {
                Id = id,
                Value = model.Description
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
            if (await _registrationReasonService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Registration reason descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _registrationReasonService.Add(new RegistrationReason
            {
                Value = model.Description
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
