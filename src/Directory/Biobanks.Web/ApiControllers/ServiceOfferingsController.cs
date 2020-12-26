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
    [RoutePrefix("api/ServiceOfferings")]
    public class ServiceOfferingsController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public ServiceOfferingsController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _biobankReadService.ListServiceOfferingsAsync())
                .Select(x =>

            Task.Run(async () => new ReadServiceOfferingModel
            {
                Id = x.ServiceId,
                Name = x.Name,
                OrganisationCount = await _biobankReadService.GetServiceOfferingOrganisationCount(x.ServiceId),
                SortOrder = x.SortOrder
            }).Result)

                .ToList();

            return models;
        }

        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListServiceOfferingsAsync()).Where(x => x.ServiceId == id).First();

            if (await _biobankReadService.IsServiceOfferingInUse(id))
            {
                return Json(new
                {
                    msg = $"The service offering \"{model.Name}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteServiceOfferingAsync(new ServiceOffering
            {
                ServiceId = model.ServiceId,
                Name = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The service offering \"{model.Name}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, Models.Shared.ServiceOfferingModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidServiceOfferingName(model.Name))
            {
                ModelState.AddModelError("ServiceOffering", "That service offering already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, prevent update
            if (await _biobankReadService.IsServiceOfferingInUse(id))
            {
                return Json(new
                {
                    msg = $"The service offering \"{model.Name}\" is currently in use, and cannot be updated.",
                    type = FeedbackMessageType.Danger
                });
            }

            // Update Service Offering
            await _biobankWriteService.UpdateServiceOfferingAsync(new ServiceOffering
            {
                ServiceId = id,
                Name = model.Name,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(Models.Shared.ServiceOfferingModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidServiceOfferingName(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Service offering names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }


            await _biobankWriteService.AddServiceOfferingAsync(new ServiceOffering
            {
                Name = model.Name,
                SortOrder = model.SortOrder
            });
            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        [HttpPut]
        [Route("Sort/{id}")]
        public async Task<IHttpActionResult> Sort(int id, Models.Shared.ServiceOfferingModel model)
        {

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateServiceOfferingAsync(new ServiceOffering
            {
                ServiceId = id,
                Name = model.Name,
                SortOrder = model.SortOrder
            },
            true);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name,
            });

        }
    }
}