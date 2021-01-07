﻿using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Entities.Data;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/MaterialType")]
    public class MaterialTypeController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public MaterialTypeController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _biobankReadService.ListMaterialTypesAsync())
                    .Select(x =>

                    Task.Run(async () => new ReadMaterialTypeModel
                    {
                        Id = x.MaterialTypeId,
                        Description = x.Description,
                        MaterialDetailCount = await _biobankReadService.GetMaterialTypeMaterialDetailCount(x.MaterialTypeId),
                        SortOrder = x.SortOrder
                    }).Result).ToList();

            return model;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(MaterialTypeModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidMaterialTypeDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Disease status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddMaterialTypeAsync(new MaterialType
            {
                MaterialTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
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
        public async Task<IHttpActionResult> Put(int id, MaterialTypeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidMaterialTypeDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("MaterialType", "That description is already in use. Material types must be unique.");
            }

            // If in use, prevent update
            if (await _biobankReadService.IsMaterialTypeInUse(id))
            {
                ModelState.AddModelError("MaterialType", $"The material type \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // Update Preservation Type
            await _biobankWriteService.UpdateMaterialTypeAsync(new MaterialType
            {
                MaterialTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

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
            var model = (await _biobankReadService.ListMaterialTypesAsync()).Where(x => x.MaterialTypeId == id).First();

            // If in use, prevent update
            if (await _biobankReadService.IsMaterialTypeInUse(id))
            {
                ModelState.AddModelError("MaterialType", $"The material type \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteMaterialTypeAsync(new MaterialType
            {
                MaterialTypeId = model.MaterialTypeId,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, MaterialTypeModel model)
        {
            await _biobankWriteService.UpdateMaterialTypeAsync(new MaterialType
            {
                MaterialTypeId = id,
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