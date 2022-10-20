using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Models.ADAC;
using Biobanks.Web.Filters;
using ServiceOfferingModel = Biobanks.Web.Models.Shared.ServiceOfferingModel;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Directory.Services;

namespace Biobanks.Web.ApiControllers
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/ServiceOffering")]
    public class ServiceOfferingController : ApiBaseController
    {
        private readonly IReferenceDataService<ServiceOffering> _serviceOfferingService;

        public ServiceOfferingController(IReferenceDataService<ServiceOffering> serviceOfferingService)
        {
            _serviceOfferingService = serviceOfferingService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _serviceOfferingService.List())
                .Select(x =>

            Task.Run(async () => new ReadServiceOfferingModel
            {
                Id = x.Id,
                Name = x.Value,
                OrganisationCount = await _serviceOfferingService.GetUsageCount(x.Id),
                SortOrder = x.SortOrder
            }).Result)

                .ToList();

            return models;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = await _serviceOfferingService.Get(id);

            // If in use, prevent update
            if (await _serviceOfferingService.IsInUse(id))
            {
                ModelState.AddModelError("ServiceOffering", $"The service offering \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _serviceOfferingService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value
            });

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, ServiceOfferingModel model)
        {
            // Validate model
            if (await _serviceOfferingService.Exists(model.Name))
            {
                ModelState.AddModelError("ServiceOffering", "That service offering already exists!");
            }

            // If in use, prevent update
            if (await _serviceOfferingService.IsInUse(id))
            {
                ModelState.AddModelError("ServiceOffering", $"The service offering \"{model.Name}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // Update Service Offering
            await _serviceOfferingService.Update(new ServiceOffering
            {
                Id = id,
                Value = model.Name,
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
            if (await _serviceOfferingService.Exists(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Service offering names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _serviceOfferingService.Add(new ServiceOffering
            {
                Value = model.Name,
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
            await _serviceOfferingService.Update(new ServiceOffering
            {
                Id = id,
                Value = model.Name,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name,
            });
        }
    }
}