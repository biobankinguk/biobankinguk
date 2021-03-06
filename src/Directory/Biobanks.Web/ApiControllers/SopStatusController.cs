﻿using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
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
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _biobankReadService.ListSopStatusesAsync())
                .Select(x =>
                    Task.Run(async () => new SopStatusModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetSopStatusUsageCount(x.Id)//GetCollectionPointUsageCount(x.CollectionPointId)
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
                Id = model.Id,
                Value = model.Description,
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
                Id = model.Id,
                Value = model.Description,
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
            var model = (await _biobankReadService.ListSopStatusesAsync()).Where(x => x.Id == id).First();

            // If in use, prevent update
            if (await _biobankReadService.IsSopStatusInUse(id))
            {
                ModelState.AddModelError("SopStatus", $"The access condition \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteSopStatusAsync(new SopStatus
            {
                Id = model.Id,
                Value = model.Value,
                SortOrder = model.SortOrder
            });

            // Success
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, SopStatusModel model)
        {
            await _biobankWriteService.UpdateSopStatusAsync(new SopStatus
            {
                Id = id,
                Value = model.Description,
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