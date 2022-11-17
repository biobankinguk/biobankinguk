using AutoMapper;
using Biobanks.Search.Legacy;
using Biobanks.Shared.Services.Contracts;
using Biobanks.Submissions.Api.Models.SuperUser;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Biobanks.Web.Controllers
{

  //[UserAuthorize(Roles = "SuperUser")] TODO: Roles
  public class SuperUserController : Controller
  {
    private readonly IOrganisationService _organisationService;
    private readonly ISampleSetService _sampleSetService;
    private readonly ICapabilityService _capabilityService;
    private readonly IBiobankWriteService _biobankWriteService;
    private readonly IBiobankIndexService _indexService;
    private readonly ISearchProvider _searchProvider;
    private readonly IMapper _mapper;

    public SuperUserController(
        IOrganisationService organisationService,
        ISampleSetService sampleSetService,
        ICapabilityService capabilityService,
        IBiobankWriteService biobankWriteService,
        IBiobankIndexService indexService,
        ISearchProvider searchProvider,
        IMapper mapper)
    {
      _organisationService = organisationService;
      _sampleSetService = sampleSetService;
      _capabilityService = capabilityService;
      _biobankWriteService = biobankWriteService;
      _indexService = indexService;
      _searchProvider = searchProvider;
      _mapper = mapper;
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

    //TODO FixorgURL

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
    public async Task<RedirectToRouteResult> ReindexAllData()
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

    public async Task<RedirectToRouteResult> BuildIndex()
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
