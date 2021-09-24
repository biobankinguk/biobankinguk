using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/Sex")]
    public class SexController : ApiBaseController
    {
        private readonly IReferenceDataService<Sex> _sexService;

        public SexController(IReferenceDataService<Sex> sexService)
        {
            _sexService = sexService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _sexService.List())
                .Select(x =>
                    Task.Run(async () => new ReadSexModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SexCount = await _sexService.GetUsageCount(x.Id),
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
            if (await _sexService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Sex descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _sexService.Add(new Sex
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
            var existing = await _sexService.Get(model.Description);

            //If this description is valid, it already exists
            if (existing != null && existing.Id != id)
            {
                ModelState.AddModelError("Description", "That description is already in use by another material type. Sex descriptions must be unique.");
            }

            if (await _sexService.IsInUse(id))
            {
                ModelState.AddModelError("Description", "This sex is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _sexService.Update(new Sex
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
            var model = await _sexService.Get(id);

            if (await _sexService.IsInUse(id))
            {
                ModelState.AddModelError("Description", $"The sex \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _sexService.Delete(id);

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
            await _sexService.Update(new Sex
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
    }
}