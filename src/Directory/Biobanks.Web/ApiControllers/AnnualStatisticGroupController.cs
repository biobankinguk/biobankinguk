using Biobanks.Services.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [Obsolete("To be deleted when the Directory core version goes live." +
        " Any changes made here will need to be made in the corresponding core version"
        , false)]
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/AnnualStatisticGroup")]
    public class AnnualStatisticGroupController : ApiBaseController
    {
        private readonly IReferenceDataService<AnnualStatisticGroup> _annualStatisticGroupService;


        public AnnualStatisticGroupController(IReferenceDataService<AnnualStatisticGroup> annualStatisticGroupService)
        {
            _annualStatisticGroupService = annualStatisticGroupService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _annualStatisticGroupService.List())
                .Select(x =>

                Task.Run(async () => new ReadAnnualStatisticGroupModel
                {
                    AnnualStatisticGroupId = x.Id,
                    Name = x.Value,
                    AnnualStatisticGroupCount = await _annualStatisticGroupService.GetUsageCount(x.Id)
                }).Result)

                .ToList();

            return model;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = await _annualStatisticGroupService.Get(id);

            if (await _annualStatisticGroupService.IsInUse(id))
            {
                ModelState.AddModelError("Name", "This annual statistic group is currently in use and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _annualStatisticGroupService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, AnnualStatisticGroupModel model)
        {
            var existing = await _annualStatisticGroupService.Get(model.Name);

            //If this name is valid, it already exists
            if (existing != null && existing.Id != id)
            {
                ModelState.AddModelError("Name", "That name is already in use by another annual statistic group. Annual Statistic Group names must be unique.");
            }

            if (await _annualStatisticGroupService.IsInUse(id))
            {
                ModelState.AddModelError("Name", "This annual statistic group is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _annualStatisticGroupService.Update(new AnnualStatisticGroup
            {
                Id = model.AnnualStatisticGroupId,
                Value = model.Name
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
            if (await _annualStatisticGroupService.Exists(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Annual Statistic Group names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _annualStatisticGroupService.Add(new AnnualStatisticGroup
            {
                Value = model.Name
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