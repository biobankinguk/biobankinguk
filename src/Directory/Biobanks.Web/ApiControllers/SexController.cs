using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Web.Filters;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/Sex")]
    public class SexController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public SexController(IBiobankReadService biobankReadService,
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
            var model = (await _biobankReadService.ListSexesAsync())
            .Select(x =>

            Task.Run(async () => new ReadSexModel
            {
                Id = x.Id,
                Description = x.Value,
                SexCount = await _biobankReadService.GetSexCount(x.Id),
                SortOrder = x.SortOrder
            }).Result)

            .ToList();

            return model;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(SexModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidSexDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Sex descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddSexAsync(new Sex
            {
                Id = model.Id,
                Value = model.Description,
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
        public async Task<IHttpActionResult> Put(int id, SexModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidSexDescriptionAsync(id, model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use by another material type. Sex descriptions must be unique.");
            }

            if (await _biobankReadService.IsSexInUse(id))
            {
                ModelState.AddModelError("Description", "This sex is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateSexAsync(new Sex
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
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
            var model = (await _biobankReadService.ListSexesAsync()).Where(x => x.Id == id).First();

            if (await _biobankReadService.IsSexInUse(id))
            {
                ModelState.AddModelError("Description", $"The sex \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteSexAsync(new Sex
            {
                Id = model.Id,
                Value = model.Value,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, SexModel model)
        {
            await _biobankWriteService.UpdateSexAsync(new Sex
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