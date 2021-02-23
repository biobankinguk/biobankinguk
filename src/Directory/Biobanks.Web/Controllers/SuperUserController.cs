using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Biobanks.Search.Legacy;
using Biobanks.Services.Contracts;
using Biobanks.Web.Models.SuperUser;
using Biobanks.Web.Utilities;
using Biobanks.Web.Filters;
using Microsoft.ApplicationInsights;

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
            // View Model (Default View Shows No Data)
            var model = new SearchIndexModel();

            // Prevent Error Propagation When Search Index Is Down
            try
            {
                model.TotalSampleSetCount = await _biobankReadService.GetSampleSetCountAsync();
                model.IndexableSampleSetCount = await _biobankReadService.GetIndexableSampleSetCountAsync();
                model.SuspendedSampleSetCount = await _biobankReadService.GetSuspendedSampleSetCountAsync();
                model.CollectionSearchDocumentCount = await _searchProvider.CountCollectionSearchDocuments();
                model.TotalCapabilityCount = await _biobankReadService.GetCapabilityCountAsync();
                model.IndexableCapabilityCount = await _biobankReadService.GetIndexableCapabilityCountAsync();
                model.SuspendedCapabilityCount = await _biobankReadService.GetSuspendedCapabilityCountAsync();
                model.CapabilitySearchDocumentCount = await _searchProvider.CountCapabilitySearchDocuments();

                // Cluster Health Status
                ViewBag.Status = await _indexService.GetClusterHealth();
            }
            catch (Exception e)
            {
                // Log Error via Application Insights
                var ai = new TelemetryClient();
                ai.TrackException(e);
            }

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
