using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Directory.Services.Contracts;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Models.ADAC;

using ServiceOfferingModel = Biobanks.Web.Models.Shared.ServiceOfferingModel;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/ServiceOffering")]
    public class ServiceOfferingController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public ServiceOfferingController(IBiobankReadService biobankReadService,
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
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListServiceOfferingsAsync()).Where(x => x.ServiceId == id).First();

            // If in use, prevent update
            if (await _biobankReadService.IsServiceOfferingInUse(id))
            {
                ModelState.AddModelError("ServiceOffering", $"The service offering \"{model.Name}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteServiceOfferingAsync(new ServiceOffering
            {
                ServiceId = model.ServiceId,
                Name = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, ServiceOfferingModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidServiceOfferingName(model.Name))
            {
                ModelState.AddModelError("ServiceOffering", "That service offering already exists!");
            }

            // If in use, prevent update
            if (await _biobankReadService.IsServiceOfferingInUse(id))
            {
                ModelState.AddModelError("ServiceOffering", $"The service offering \"{model.Name}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
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
        public async Task<IHttpActionResult> Post(ServiceOfferingModel model)
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

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, ServiceOfferingModel model)
        {
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