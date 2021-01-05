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
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _biobankReadService.ListSexesAsync())
            .Select(x =>

            Task.Run(async () => new ReadSexModel
            {
                Id = x.SexId,
                Description = x.Description,
                SexCount = await _biobankReadService.GetSexCount(x.SexId),
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
                SexId = model.Id,
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
                SexId = id,
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

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListSexesAsync()).Where(x => x.SexId == id).First();

            if (await _biobankReadService.IsSexInUse(id))
            {
                ModelState.AddModelError("Description", $"The sex \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteSexAsync(new Sex
            {
                SexId = model.SexId,
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
        public async Task<IHttpActionResult> Move(int id, SexModel model)
        {
            await _biobankWriteService.UpdateSexAsync(new Sex
            {
                SexId = id,
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