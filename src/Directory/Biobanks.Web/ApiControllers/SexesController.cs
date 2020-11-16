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
    public class SexesController : ApiController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public SexesController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        
        // GET: Sexes
        [HttpGet]
        public async Task<IList> Sexes()
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
        public async Task<IHttpActionResult> AddSexAjax(SexModel model)
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
                redirect = $"AddSexSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditSexAjax(SexModel model, bool sortOnly = false)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidSexDescriptionAsync(model.Id, model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use by another material type. Sex descriptions must be unique.");
            }

            if (await _biobankReadService.IsSexInUse(model.Id))
            {
                ModelState.AddModelError("Description", "This sex is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateSexAsync(new Sex
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
                redirect = $"EditSexSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteSex(SexModel model)
        {
            if (await _biobankReadService.IsSexInUse(model.Id))
            {
            return Json(new
            {
                msg = $"The sex \"{model.Description}\" is currently in use, and cannot be deleted.",
                type = FeedbackMessageType.Danger
            });
            }

            await _biobankWriteService.DeleteSexAsync(new Sex
            {
                SexId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The sex \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }


        private IHttpActionResult JsonModelInvalidResponse(ModelStateDictionary state)
        {
            return Json(new
            {
                success = false,
                errors = state.Values
                    .Where(x => x.Errors.Count > 0)
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }
    }
}