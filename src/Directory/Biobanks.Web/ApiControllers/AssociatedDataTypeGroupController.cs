using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;
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
    [RoutePrefix("api/AssociatedDataTypeGroup")]
    public class AssociatedDataTypeGroupController : ApiBaseController
    {
        private readonly IReferenceDataService<AssociatedDataTypeGroup> _associatedDataTypeGroupService;

        public AssociatedDataTypeGroupController(IReferenceDataService<AssociatedDataTypeGroup> associatedDataTypeGroupService)
        {
            _associatedDataTypeGroupService = associatedDataTypeGroupService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _associatedDataTypeGroupService.List())
                .Select(x =>
                    Task.Run(async () => new ReadAssociatedDataTypeGroupModel
                    {
                        AssociatedDataTypeGroupId = x.Id,
                        Name = x.Value,
                        AssociatedDataTypeGroupCount = await _associatedDataTypeGroupService.GetUsageCount(x.Id)
                    })
                    .Result
                 )
                .ToList();

            return model;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = await _associatedDataTypeGroupService.Get(id);

            if (await _associatedDataTypeGroupService.IsInUse(id))
            {
                ModelState.AddModelError("Name", $"The associated data type group \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _associatedDataTypeGroupService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value
            });
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(AssociatedDataTypeGroupModel model)
        {
            //If this name is valid, it already exists
            if (await _associatedDataTypeGroupService.Exists(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Associated Data Type Group names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _associatedDataTypeGroupService.Add(new AssociatedDataTypeGroup
            {
                Value = model.Name
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
        public async Task<IHttpActionResult> Put(int id, AssociatedDataTypeGroupModel model)
        {
            var exisiting = await _associatedDataTypeGroupService.Get(model.Name);

            //If this name is valid, it already exists
            if (exisiting != null && exisiting.Id != id)
            {
                ModelState.AddModelError("Name", "That name is already in use by another asscoiated data type group. Associated Data Type Group names must be unique.");
            }

            if (await _associatedDataTypeGroupService.IsInUse(id))
            {
                ModelState.AddModelError("Name", "This associated data type group is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _associatedDataTypeGroupService.Update(new AssociatedDataTypeGroup
            {
                Id = id,
                Value = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }
    }
}