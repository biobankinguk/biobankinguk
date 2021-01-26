using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;

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
                    AssociatedDataTypeGroupId = x.Id,
                    Name = x.Value,
                })
                .ToList();
            var model = (await _biobankReadService.ListAssociatedDataTypesAsync()).Select(x =>

            Task.Run(async () => new AssociatedDataTypeModel
            {
                Id = x.Id,
                Name = x.Value,
                Message = x.Message,
                CollectionCapabilityCount = await _biobankReadService.GetAssociatedDataTypeCollectionCapabilityCount(x.Id),
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
            var model = (await _biobankReadService.ListAssociatedDataTypesAsync()).Where(x => x.Id == id).First();

            if (await _biobankReadService.IsAssociatedDataTypeInUse(id))
            {
                ModelState.AddModelError("AssociatedDataTypes", $"The associated data type \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteAssociatedDataTypeAsync(new AssociatedDataType
            {
                Id = id,
                Value = model.Value
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
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
                Id = id,
                AssociatedDataTypeGroupId = model.AssociatedDataTypeGroupId,
                Value = model.Name,
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
                Id = model.Id,
                AssociatedDataTypeGroupId = model.AssociatedDataTypeGroupId,
                Value = model.Name,
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