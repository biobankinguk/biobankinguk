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
    public class AssociatedDataTypesController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AssociatedDataTypesController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        // GET: AssociatedDataTypes;
        [HttpGet]
        public async Task<IHttpActionResult> AssociatedDataTypes()
        {
            var groups = (await _biobankReadService.ListAssociatedDataTypeGroupsAsync())
                .Select(x => new AssociatedDataTypeGroupModel
                {
                    AssociatedDataTypeGroupId = x.AssociatedDataTypeGroupId,
                    Name = x.Description,
                })
                .ToList();
            var model = (await _biobankReadService.ListAssociatedDataTypesAsync()).Select(x =>

            Task.Run(async () => new AssociatedDataTypeModel
            {
                Id = x.AssociatedDataTypeId,
                Name = x.Description,
                Message = x.Message,
                CollectionCapabilityCount = await _biobankReadService.GetAssociatedDataTypeCollectionCapabilityCount(x.AssociatedDataTypeId),
                AssociatedDataTypeGroupId = x.AssociatedDataTypeGroupId,
                AssociatedDataTypeGroupName = groups.Where(y => y.AssociatedDataTypeGroupId == x.AssociatedDataTypeGroupId).FirstOrDefault()?.Name,

            }).Result)

               .ToList();

            return Json(new 
            {
                AssociatedDataTypes = model,
                AssociatedDataTypeGroups = groups
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteAssociatedDataType(AssociatedDataTypeModel model)
        {
            if (await _biobankReadService.IsAssociatedDataTypeInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The associated data type \"{model.Name}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteAssociatedDataTypeAsync(new AssociatedDataType
            {
                AssociatedDataTypeId = model.Id,
                Description = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The associated data type \"{model.Name}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditAssociatedDataTypeAjax(AssociatedDataTypeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAssociatedDataTypeDescriptionAsync(model.Name))
            {
                ModelState.AddModelError("AssociatedDataTypes", "That associated data type already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsAssociatedDataTypeInUse(model.Id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This associated data type is currently in use and cannot be edited." }
                });
            }

            var associatedDataTypes = new AssociatedDataType
            {
                AssociatedDataTypeId = model.Id,
                AssociatedDataTypeGroupId = model.AssociatedDataTypeGroupId,
                Description = model.Name,
                Message = model.Message

            };

            await _biobankWriteService.UpdateAssociatedDataTypeAsync(associatedDataTypes);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
                redirect = $"EditAssociatedDataTypeSuccess?name={model.Name}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddAssociatedDataTypeAjax(AssociatedDataTypeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAssociatedDataTypeDescriptionAsync(model.Name))
            {
                ModelState.AddModelError("AssociatedDataTypes", "That name is already in use. Associated Data Type names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var associatedDataType = new AssociatedDataType
            {
                AssociatedDataTypeId = model.Id,
                AssociatedDataTypeGroupId = model.AssociatedDataTypeGroupId,
                Description = model.Name,
                Message = model.Message
            };

            await _biobankWriteService.AddAssociatedDataTypeAsync(associatedDataType);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
                redirect = $"AddAssociatedDataTypeSuccess?name={model.Name}"
            });
        }
    }
}