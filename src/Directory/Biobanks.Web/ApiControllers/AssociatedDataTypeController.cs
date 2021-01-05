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
    [RoutePrefix("api/AssociatedDataType")]
    public class AssociatedDataTypeController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AssociatedDataTypeController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Get()
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

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListAssociatedDataTypesAsync()).Where(x => x.AssociatedDataTypeId == id).First();

            if (await _biobankReadService.IsAssociatedDataTypeInUse(id))
            {
                ModelState.AddModelError("AssociatedDataTypes", $"The associated data type \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteAssociatedDataTypeAsync(new AssociatedDataType
            {
                AssociatedDataTypeId = id,
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
        public async Task<IHttpActionResult> Put(int id, AssociatedDataTypeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAssociatedDataTypeDescriptionAsync(model.Name))
            {
                ModelState.AddModelError("AssociatedDataTypes", "That associated data type already exists!");
            }

            if (await _biobankReadService.IsAssociatedDataTypeInUse(id))
            {
                ModelState.AddModelError("AssociatedDataTypes", "This associated data type is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var associatedDataTypes = new AssociatedDataType
            {
                AssociatedDataTypeId = id,
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
            });
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(AssociatedDataTypeModel model)
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
            });
        }
    }
}