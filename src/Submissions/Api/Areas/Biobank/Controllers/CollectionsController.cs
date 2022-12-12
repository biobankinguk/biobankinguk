using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Shared.Services.Contracts;
using Biobanks.Submissions.Api.Areas.Biobank.Models;
using Biobanks.Submissions.Api.Services.Directory;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Areas.Biobank.Controllers;
public class CollectionsController : Controller
{
  private readonly IReferenceDataService<CollectionPercentage> _collectionPercentageService;
  private readonly IConfigService _configService;

  public async Task<ActionResult> CollectionPercentages()
  {
    var models = (await _collectionPercentageService.List())
        .Select(x =>
            Task.Run(async () => new CollectionPercentageModel()
            {
              Id = x.Id,
              Description = x.Value,
              SortOrder = x.SortOrder,
              LowerBound = x.LowerBound,
              UpperBound = x.UpperBound,
              SampleSetsCount = await _collectionPercentageService.GetUsageCount(x.Id)
            })
            .Result
        )
        .ToList();
    if (await _configService.GetFlagConfigValue("site.display.preservation.percent") == true)
    {
      return View(new CollectionPercentagesModel()
      {
        CollectionPercentages = models
      });
    }
    else
    {
      return RedirectToAction("LockedRef");
    }
  }
}
