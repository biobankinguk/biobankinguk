using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/SopStatus")]
    public class SopStatusController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public SopStatusController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _biobankReadService.ListSopStatusesAsync())
                .Select(x =>
                    Task.Run(async () => new SopStatusModel()
                    {
                        Id = x.SopStatusId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetSopStatusUsageCount(x.SopStatusId)//GetCollectionPointUsageCount(x.CollectionPointId)
                    })
                    .Result
                )
                .ToList();
            return models;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(SopStatusModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidSopStatusAsync(model.Description))
            {
                ModelState.AddModelError("SopStatus", "That description is already in use. Sop status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var status = new SopStatus
            {
                SopStatusId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddSopStatusAsync(status);
            await _biobankWriteService.UpdateSopStatusAsync(status, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, SopStatusModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidSopStatusAsync(model.Description))
            {
                ModelState.AddModelError("SopStatus", "That sop status already exists!");
            }

            // If in use, prevent update
            if (model.SampleSetsCount > 0)
            {
                ModelState.AddModelError("SopStatus", $"The access condition \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateSopStatusAsync(new SopStatus
            {
                SopStatusId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListSopStatusesAsync()).Where(x => x.SopStatusId == id).First();

            // If in use, prevent update
            if (await _biobankReadService.IsSopStatusInUse(id))
            {
                ModelState.AddModelError("SopStatus", $"The access condition \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteSopStatusAsync(new SopStatus
            {
                SopStatusId = model.SopStatusId,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            // Success
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, SopStatusModel model)
        {
            await _biobankWriteService.UpdateSopStatusAsync(new SopStatus
            {
                SopStatusId = id,
                Description = model.Description,
                SortOrder = model.SortOrder
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