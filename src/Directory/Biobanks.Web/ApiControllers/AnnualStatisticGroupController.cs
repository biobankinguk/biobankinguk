using Directory.Services.Contracts;
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
    [RoutePrefix("api/AnnualStatisticGroup")]
    public class AnnualStatisticGroupController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AnnualStatisticGroupController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
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

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListAnnualStatisticGroupsAsync()).Where(x => x.AnnualStatisticGroupId == id).First();

            if (await _biobankReadService.IsAnnualStatisticGroupInUse(id))
            {
                ModelState.AddModelError("Name", "This annual statistic group is currently in use and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteAnnualStatisticGroupAsync(new AnnualStatisticGroup
            {
                AnnualStatisticGroupId = id,
                Name = model.Name
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
        public async Task<IHttpActionResult> Put(int id, AnnualStatisticGroupModel model)
        {
            //If this name is valid, it already exists
            if (await _biobankReadService.ValidAnnualStatisticGroupNameAsync(id, model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use by another annual statistic group. Annual Statistic Group names must be unique.");
            }

            if (await _biobankReadService.IsAnnualStatisticGroupInUse(id))
            {
                ModelState.AddModelError("Name", "This annual statistic group is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
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
        [Route("")]
        public async Task<IHttpActionResult> Post(AnnualStatisticGroupModel model)
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