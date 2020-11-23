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
    public class AnnualStatisticGroupsController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AnnualStatisticGroupsController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        // GET: AnnualStatisticGroups
        [HttpGet]
        public async Task<IList> AnnualStatisticGroups()
        {
            var model = (await _biobankReadService.ListAnnualStatisticGroupsAsync())
                .Select(x =>

                Task.Run(async () => new ReadAnnualStatisticGroupModel
                {
                    AnnualStatisticGroupId = x.AnnualStatisticGroupId,
                    Name = x.Name,
                    AnnualStatisticGroupCount = await _biobankReadService.GetAnnualStatisticAnnualStatisticGroupCount(x.AnnualStatisticGroupId)
                }).Result)

                .ToList();

            return model;
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteAnnualStatisticGroup(AnnualStatisticGroupModel model)
        {
            if (await _biobankReadService.IsAnnualStatisticGroupInUse(model.AnnualStatisticGroupId))
            {
                return Json(new
                {
                    msg = $"The annual statistic group \"{model.Name}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteAnnualStatisticGroupAsync(new AnnualStatisticGroup
            {
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Name = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The annual statistic group \"{model.Name}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditAnnualStatisticGroupAjax(AnnualStatisticGroupModel model)
        {
            //If this name is valid, it already exists
            if (await _biobankReadService.ValidAnnualStatisticGroupNameAsync(model.AnnualStatisticGroupId, model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use by another annual statistic group. Annual Statistic Group names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsAnnualStatisticGroupInUse(model.AnnualStatisticGroupId))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This annual statistic group is currently in use and cannot be edited." }
                });
            }

            await _biobankWriteService.UpdateAnnualStatisticGroupAsync(new AnnualStatisticGroup
            {
                AnnualStatisticGroupId = model.AnnualStatisticGroupId,
                Name = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddAnnualStatisticGroupAjax(AnnualStatisticGroupModel model)
        {
            //If this name is valid, it already exists
            if (await _biobankReadService.ValidAnnualStatisticGroupNameAsync(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Annual Statistic Group names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddAnnualStatisticGroupAsync(new AnnualStatisticGroup
            {
                Name = model.Name
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