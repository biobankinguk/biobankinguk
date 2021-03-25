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
using System.IO;
using System.Net.Http;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using Biobanks.Directory.Data;
using Biobanks.Directory.Data.Transforms.Url;

namespace Biobanks.Web.Controllers
{
    [UserAuthorize(Roles = "SuperUser")]
    public class SuperUserController : ApplicationBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;
        private readonly IBiobankIndexService _indexService;
        private readonly ISearchProvider _searchProvider;

        public SuperUserController(
            IBiobankReadService biobankReadService,
            IBiobankWriteService biobankWriteService,
            IBiobankIndexService indexService,
            ISearchProvider searchProvider)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
            _indexService = indexService;
            _searchProvider = searchProvider;
        }

        // GET: SuperUser
        public ActionResult Index()
        {
            return View();
        }

        #region Super User Tools

        public ActionResult Tools()
        {
            return View();
        }

        public async Task<RedirectToRouteResult> FixOrgURL()
        {
            try
            {
                var organisations =  _biobankReadService.GetOrganisations();
                foreach (var organisation in organisations)
                {
                    await _biobankWriteService.UpdateOrganisationURLAsync(organisation.OrganisationId);
                }
                
            }
            catch (Exception e) when (e is HttpRequestException || e is DbUpdateException)
            {
                SetTemporaryFeedbackMessage($"The process failed to succesfully complete due to: {e.GetType().Name}.", FeedbackMessageType.Warning);
                return RedirectToAction("Tools");
            }

            SetTemporaryFeedbackMessage("The process of fixing any broken organisation URLs has succesfully completed.", FeedbackMessageType.Success);
            return RedirectToAction("Tools");
        }

        #endregion

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

        public async Task<RedirectToRouteResult> BuildIndex()
        {
            try
            {
                await _indexService.BuildIndex();
            }
            catch (Exception e) when (e is IOException || e is HttpRequestException)
            {          
                SetTemporaryFeedbackMessage($"The building process failed to succesfully complete due to: {e.GetType().Name}.", FeedbackMessageType.Warning);
                return RedirectToAction("SearchIndex");
            }
            //The reindex method is called to populate the index after creation
            await ReindexAllData();
            SetTemporaryFeedbackMessage("The building of the index has begun. Pending jobs can be viewed in the Hangfire dashboard.", FeedbackMessageType.Info);
            return RedirectToAction("SearchIndex");
        }

        #endregion
    }
}
