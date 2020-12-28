using System.Threading.Tasks;
using System.Web.Http;
using Directory.Entity.Data;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Utilities;
using Biobanks.Web.Models.ADAC;
using System.Web.Http.Results;
using System.Collections;
using System.Web.Http.ModelBinding;
using Directory.Services.Contracts;
using System.Linq;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/AssociatedDataProcurementTimeFrame")]
    public class AssociatedDataProcurementTimeFrameController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AssociatedDataProcurementTimeFrameController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _biobankReadService.ListAssociatedDataProcurementTimeFrames())
                    .Select(x =>

                Task.Run(async () => new ReadAssociatedDataProcurementTimeFrameModel
                {
                    Id = x.AssociatedDataProcurementTimeframeId,
                    Description = x.Description,
                    DisplayName = x.DisplayValue,
                    CollectionCapabilityCount = await _biobankReadService.GetAssociatedDataProcurementTimeFrameCollectionCapabilityCount(x.AssociatedDataProcurementTimeframeId),
                    SortOrder = x.SortOrder
                }).Result).ToList();

            return models;

        }

        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListAssociatedDataProcurementTimeFrames()).Where(x => x.AssociatedDataProcurementTimeframeId == id).First();

            //Validate min amount of time frames
            var timeFrames = await _biobankReadService.ListAssociatedDataProcurementTimeFrames();
            if (timeFrames.Count() <= 2)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"A minimum amount of 2 time frames are allowed.");
            }

            if (await _biobankReadService.IsAssociatedDataProcurementTimeFrameInUse(id))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"The associated data procurement time frame \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteAssociatedDataProcurementTimeFrameAsync(new AssociatedDataProcurementTimeframe
            {
                AssociatedDataProcurementTimeframeId = id,
                Description = model.Description
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, Models.Shared.AssociatedDataProcurementTimeFrameModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAssociatedDataProcurementTimeFrameDescriptionAsync(id, model.Description))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "That Associated Data Procurement Time Frame already exists!");
            }

            if (model.DisplayName == null)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "The Display Name field is required.");
            }

            if (model.DisplayName.Length > 10)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "The Display Name field allows a maximum of 10 characters.");
            }

            // If in use, prevent update
            if (await _biobankReadService.IsAssociatedDataProcurementTimeFrameInUse(id))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"The Associated Data Procurement Time Frame \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // Update Preservation Type
            await _biobankWriteService.UpdateAssociatedDataProcurementTimeFrameAsync(new AssociatedDataProcurementTimeframe
            {
                AssociatedDataProcurementTimeframeId = id,
                Description = model.Description,
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
        public async Task<IHttpActionResult> Post(Models.Shared.AssociatedDataProcurementTimeFrameModel model)
        {
            // Validate model
            var timeFrames = await _biobankReadService.ListAssociatedDataProcurementTimeFrames();
            if (timeFrames.Count() >= 5)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", $"A maximum amount of 5 time frames are allowed.");
            }

            if (await _biobankReadService.ValidAssociatedDataProcurementTimeFrameDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "That description is already in use. Associated Data Procurement Time Frame descriptions must be unique.");
            }

            if (model.DisplayName == null)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "The Display Name field is required.");
            }

            if (model.DisplayName.Length > 10)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "The Display Name field allows a maximum of 10 characters.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var procurement = new AssociatedDataProcurementTimeframe
            {
                AssociatedDataProcurementTimeframeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                DisplayValue = model.DisplayName
            };

            await _biobankWriteService.AddAssociatedDataProcurementTimeFrameAsync(procurement);
            await _biobankWriteService.UpdateAssociatedDataProcurementTimeFrameAsync(procurement, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        [HttpPut]
        [Route("Sort/{id}")]
        public async Task<IHttpActionResult> Sort(int id, Models.Shared.AssociatedDataProcurementTimeFrameModel model)
        {
            // Update Preservation Type
            await _biobankWriteService.UpdateAssociatedDataProcurementTimeFrameAsync(new AssociatedDataProcurementTimeframe
            {
                AssociatedDataProcurementTimeframeId = id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                DisplayValue = model.DisplayName
            },
            true);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });

        }

    }
}