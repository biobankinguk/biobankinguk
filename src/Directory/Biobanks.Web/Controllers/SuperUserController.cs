using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Biobanks.Search.Legacy;
using Directory.Services.Contracts;
using Biobanks.Web.Models.SuperUser;
using Biobanks.Web.Utilities;
using Biobanks.Web.Filters;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;

namespace Biobanks.Web.Controllers
{
    [UserAuthorize(Roles = "SuperUser")]
    public class SuperUserController : ApplicationBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankIndexService _indexService;
        private readonly ISearchProvider _searchProvider;

        public SuperUserController(
            IBiobankReadService biobankReadService,
            IBiobankIndexService indexService,
            ISearchProvider searchProvider)
        {
            _biobankReadService = biobankReadService;
            _indexService = indexService;
            _searchProvider = searchProvider;
        }

        // GET: SuperUser
        public ActionResult Index()
        {
            return View();
        }

        #region Search Index Management

        [HttpGet]
        public async Task<ViewResult> SearchIndex()
        {
            // Cluster Health Status
            ViewBag.Status = await _indexService.GetClusterHealth();

            // Index Sample Counts
            var model = new SearchIndexModel
            {
                TotalSampleSetCount = await _biobankReadService.GetSampleSetCountAsync(),
                IndexableSampleSetCount = await _biobankReadService.GetIndexableSampleSetCountAsync(),
                SuspendedSampleSetCount = await _biobankReadService.GetSuspendedSampleSetCountAsync(),
                CollectionSearchDocumentCount = await _searchProvider.CountCollectionSearchDocuments(),
                TotalCapabilityCount = await _biobankReadService.GetCapabilityCountAsync(),
                IndexableCapabilityCount = await _biobankReadService.GetIndexableCapabilityCountAsync(),
                SuspendedCapabilityCount = await _biobankReadService.GetSuspendedCapabilityCountAsync(),
                CapabilitySearchDocumentCount = await _searchProvider.CountCapabilitySearchDocuments()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<RedirectToRouteResult> ReindexAllData()
        {
            //first empty the index
            await _indexService.ClearIndex();

            //Now repopulate it
            var sampleSetIds = await _biobankReadService.GetAllSampleSetIdsAsync();

            await _indexService.BulkIndexSampleSets(sampleSetIds.ToList());

            var capabilityIds = await _biobankReadService.GetAllCapabilityIdsAsync();

            await _indexService.BulkIndexCapabilities(capabilityIds.ToList());

            SetTemporaryFeedbackMessage("The GREAT REINDEXING has begun. Pending jobs can be viewed in the Hangfire dashboard.", FeedbackMessageType.Info);

            return RedirectToAction("SearchIndex");
        }
        #endregion
    }
}
