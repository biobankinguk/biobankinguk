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
    public class AssociatedDataTypeGroupsController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AssociatedDataTypeGroupsController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        // GET: AssociatedDataTypeGroups
        [HttpGet]
        public async Task<IList> AssociatedDataTypeGroups()
        {
            var model = (await _biobankReadService.ListAssociatedDataTypeGroupsAsync())
                    .Select(x =>

                    Task.Run(async () => new ReadAssociatedDataTypeGroupModel
                    {
                        AssociatedDataTypeGroupId = x.AssociatedDataTypeGroupId,
                        Name = x.Description,
                        AssociatedDataTypeGroupCount = await _biobankReadService.GetAssociatedDataTypeGroupCount(x.AssociatedDataTypeGroupId)
                    }).Result)

                    .ToList();

            return model;
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteAssociatedDataTypeGroup(AssociatedDataTypeGroupModel model)
        { 
            if (await _biobankReadService.IsAssociatedDataTypeGroupInUse(model.AssociatedDataTypeGroupId))
            {
                return Json(new
                {
                    msg = $"The associated data type group \"{model.Name}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteAssociatedDataTypeGroupAsync(new Directory.Entity.Data.AssociatedDataTypeGroup
            {
                AssociatedDataTypeGroupId = model.AssociatedDataTypeGroupId,
                Description = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The associated data type group \"{model.Name}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddAssociatedDataTypeGroupAjax(AssociatedDataTypeGroupModel model)
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

            await _biobankWriteService.AddAssociatedDataTypeGroupAsync(new Directory.Entity.Data.AssociatedDataTypeGroup
            {
                Description = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditAssociatedDataTypeGroupAjax(AssociatedDataTypeGroupModel model)
        {
            //If this name is valid, it already exists
            if (await _biobankReadService.ValidAssociatedDataTypeGroupNameAsync(model.AssociatedDataTypeGroupId, model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use by another asscoiated data type group. Associated Data Type Group names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsAssociatedDataTypeGroupInUse(model.AssociatedDataTypeGroupId))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This associated data type group is currently in use and cannot be edited." }
                });
            }

            await _biobankWriteService.UpdateAssociatedDataTypeGroupAsync(new Directory.Entity.Data.AssociatedDataTypeGroup
            {
                AssociatedDataTypeGroupId = model.AssociatedDataTypeGroupId,
                Description = model.Name
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