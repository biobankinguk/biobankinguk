using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.Shared;
using System.Collections;
using Biobanks.Services.Contracts;
using System.Linq;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;
using System;

namespace Biobanks.Web.ApiControllers
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]

    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/AssociatedDataProcurementTimeFrame")]
    public class AssociatedDataProcurementTimeFrameController : ApiBaseController
    {
        private readonly IReferenceDataService<AssociatedDataProcurementTimeframe> _associatedDataProcurementTimeFrameService;

        public AssociatedDataProcurementTimeFrameController(
            IReferenceDataService<AssociatedDataProcurementTimeframe> associatedDataProcurementTimeFrameService)
        {
            _associatedDataProcurementTimeFrameService = associatedDataProcurementTimeFrameService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _associatedDataProcurementTimeFrameService.List())
                .Select(x =>
                    Task.Run(async () => new Models.ADAC.ReadAssociatedDataProcurementTimeFrameModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        DisplayName = x.DisplayValue,
                        CollectionCapabilityCount = await _associatedDataProcurementTimeFrameService.GetUsageCount(x.Id),
                        SortOrder = x.SortOrder
                    })
                    .Result
                )
                .ToList();

            return models;

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = await _associatedDataProcurementTimeFrameService.Get(id);

            //Validate min amount of time frames
            if (await _associatedDataProcurementTimeFrameService.Count() <= 2)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"A minimum amount of 2 time frames are allowed.");
            }

            if (await _associatedDataProcurementTimeFrameService.IsInUse(id))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"The associated data procurement time frame \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _associatedDataProcurementTimeFrameService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, AssociatedDataProcurementTimeFrameModel model)
        {
            // Validate model
            var exisiting = await _associatedDataProcurementTimeFrameService.Get(model.Description);
            
            if (exisiting != null && exisiting.Id != id)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "That Associated Data Procurement Time Frame already exists!");
            }

            // If in use, prevent update
            if (await _associatedDataProcurementTimeFrameService.IsInUse(id))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"The Associated Data Procurement Time Frame \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _associatedDataProcurementTimeFrameService.Update(new AssociatedDataProcurementTimeframe
            {
                Id = id,
                Value = model.Description,
                DisplayValue = model.DisplayName,
                SortOrder = model.SortOrder
            });

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(AssociatedDataProcurementTimeFrameModel model)
        {
            // Validate model
            if (await _associatedDataProcurementTimeFrameService.Count() >= 5)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"A maximum amount of 5 time frames are allowed.");
            }

            if (await _associatedDataProcurementTimeFrameService.Exists(model.Description))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "That description is already in use. Associated Data Procurement Time Frame descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var procurement = new AssociatedDataProcurementTimeframe
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                DisplayValue = model.DisplayName
            };

            await _associatedDataProcurementTimeFrameService.Add(procurement);
            await _associatedDataProcurementTimeFrameService.Update(procurement);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, AssociatedDataProcurementTimeFrameModel model)
        {
            await _associatedDataProcurementTimeFrameService.Update(new AssociatedDataProcurementTimeframe
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                DisplayValue = model.DisplayName
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });

        }

    }
}