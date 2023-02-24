using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Biobanks.Data.Transforms.Url;
using Biobanks.Search.Legacy;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Models.SuperUser;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Api.Controllers.Directory
{

  [Authorize(nameof(AuthPolicies.IsSuperUser))]
  public class SuperUserController : Controller
  {
    private readonly IOrganisationDirectoryService _organisationService;
    private readonly ISampleSetService _sampleSetService;
    private readonly ICapabilityService _capabilityService;
    private readonly IBiobankIndexService _indexService;
    private readonly ISearchProvider _searchProvider;

    public SuperUserController(
        IOrganisationDirectoryService organisationService,
        ISampleSetService sampleSetService,
        ICapabilityService capabilityService,
        IBiobankIndexService indexService,
        ISearchProvider searchProvider)
    {
      _organisationService = organisationService;
      _sampleSetService = sampleSetService;
      _capabilityService = capabilityService;
      _indexService = indexService;
      _searchProvider = searchProvider;
    }

    // GET: SuperUser
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult Tools()
    {
      return View();
    }
    
    public async Task<RedirectToActionResult> FixOrgURL()
    {
      try
      {
        var organisations = await _organisationService.List();
                
        foreach (var organisation in organisations)
        {
          // Update URL
          organisation.Url = UrlTransformer.Transform(organisation.Url);

          await _organisationService.Update(organisation);
        }
                
      }
      catch (Exception e) when (e is HttpRequestException || e is DbUpdateException)
      {
        this.SetTemporaryFeedbackMessage($"The process failed to succesfully complete due to: {e.GetType().Name}.", FeedbackMessageType.Warning);
        return RedirectToAction("Tools");
      }

      this.SetTemporaryFeedbackMessage("The process of fixing any broken organisation URLs has succesfully completed.", FeedbackMessageType.Success);
      return RedirectToAction("Tools");
    }

    [HttpGet]
    public async Task<ViewResult> SearchIndex()
    {
      // View Model (Default View Shows No Data)
      var model = new SearchIndexModel
      {
        // Prevent Error Propagation When Search Index Is Down

        TotalSampleSetCount = await _sampleSetService.GetSampleSetCountAsync(),
        IndexableSampleSetCount = await _sampleSetService.GetIndexableSampleSetCountAsync(),
        SuspendedSampleSetCount = await _sampleSetService.GetSuspendedSampleSetCountAsync(),
        CollectionSearchDocumentCount = await _searchProvider.CountCollectionSearchDocuments(),
        TotalCapabilityCount = await _capabilityService.GetCapabilityCountAsync(),
        IndexableCapabilityCount = await _capabilityService.GetIndexableCapabilityCountAsync(),
        SuspendedCapabilityCount = await _capabilityService.GetSuspendedCapabilityCountAsync(),
        CapabilitySearchDocumentCount = await _searchProvider.CountCapabilitySearchDocuments()
      };

      // Cluster Health Status
      ViewBag.Status = await _indexService.GetClusterHealth();
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ReindexAllData()
    {
      //first empty the index
      await _indexService.ClearIndex();

      //Now repopulate it
      var sampleSetIds = await _sampleSetService.GetAllSampleSetIdsAsync();

      await _indexService.BulkIndexSampleSets(sampleSetIds.ToList());

      var capabilityIds = await _capabilityService.GetAllCapabilityIdsAsync();

      await _indexService.BulkIndexCapabilities(capabilityIds.ToList());

      this.SetTemporaryFeedbackMessage("The GREAT REINDEXING has begun. Pending jobs can be viewed in the Hangfire dashboard.", FeedbackMessageType.Info);

      return RedirectToAction("SearchIndex");
    }

    public async Task<IActionResult> BuildIndex()
    {
      try
      {
        await _indexService.BuildIndex();
      }
      catch (Exception e) when (e is IOException || e is HttpRequestException)
      {
        this.SetTemporaryFeedbackMessage($"The building process failed to succesfully complete due to: {e.GetType().Name}.", FeedbackMessageType.Warning);
        return RedirectToAction("SearchIndex");
      }
      //The reindex method is called to populate the index after creation
      await ReindexAllData();
      this.SetTemporaryFeedbackMessage("The building of the index has begun. Pending jobs can be viewed in the Hangfire dashboard.", FeedbackMessageType.Info);
      return RedirectToAction("SearchIndex");
    }
  }
}
