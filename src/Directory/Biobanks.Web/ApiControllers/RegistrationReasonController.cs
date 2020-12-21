using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Directory.Entity.Data;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Utilities;
using Biobanks.Web.Models.ADAC;
using System.Web.Http.Results;
using System.Collections;
using System.Web.Http.ModelBinding;

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
        [Route("")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListRegistrationReasonsAsync()).Where(x => x.RegistrationReasonId == id).First();

            if (await _biobankReadService.IsRegistrationReasonInUse(id))
            {
                return Json(new
                {
                    msg = $"The registration reason \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteRegistrationReasonAsync(new RegistrationReason
            {
                RegistrationReasonId = model.RegistrationReasonId,
                Description = model.Description
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The registration reason \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, Models.Shared.RegistrationReasonModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidRegistrationReasonDescriptionAsync(id, model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use by another registration reason. Registration reason descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsRegistrationReasonInUse(id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This registration reason is currently in use and cannot be edited." }
                });
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
        public async Task<IHttpActionResult> Post(Models.Shared.RegistrationReasonModel model)
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