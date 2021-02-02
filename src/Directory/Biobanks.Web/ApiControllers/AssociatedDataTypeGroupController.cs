using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/AssociatedDataTypeGroup")]
    public class AssociatedDataTypeGroupController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AssociatedDataTypeGroupController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _biobankReadService.ListAssociatedDataTypeGroupsAsync())
                    .Select(x =>

                    Task.Run(async () => new ReadAssociatedDataTypeGroupModel
                    {
                        AssociatedDataTypeGroupId = x.Id,
                        Name = x.Value,
                        AssociatedDataTypeGroupCount = await _biobankReadService.GetAssociatedDataTypeGroupCount(x.Id)
                    }).Result)

                    .ToList();

            return model;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListAssociatedDataTypeGroupsAsync()).Where(x => x.Id == id).First();

            if (await _biobankReadService.IsAssociatedDataTypeGroupInUse(id))
            {
                ModelState.AddModelError("Name", $"The associated data type group \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteAssociatedDataTypeGroupAsync(new AssociatedDataTypeGroup
            {
                Id = model.Id,
                Value = model.Value
            });

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
            if (await _biobankReadService.ValidAssociatedDataTypeGroupNameAsync(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Associated Data Type Group names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddAssociatedDataTypeGroupAsync(new AssociatedDataTypeGroup
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
            //If this name is valid, it already exists
            if (await _biobankReadService.ValidAssociatedDataTypeGroupNameAsync(id, model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use by another asscoiated data type group. Associated Data Type Group names must be unique.");
            }

            if (await _biobankReadService.IsAssociatedDataTypeGroupInUse(id))
            {
                ModelState.AddModelError("Name", "This associated data type group is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateAssociatedDataTypeGroupAsync(new AssociatedDataTypeGroup
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