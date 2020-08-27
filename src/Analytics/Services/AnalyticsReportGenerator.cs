using System.Collections.Generic;
using Analytics.Services.Contracts;
using System.Threading.Tasks;
using Analytics.Services.Dto;
using System.Linq;

namespace Analytics.Services
{
    public class AnalyticsReportGenerator : IAnalyticsReportGenerator
    {
        private readonly int numOfTopBiobanks = 10;
        private readonly IGoogleAnalyticsReadService _googleAnalyticsReadService;

        public AnalyticsReportGenerator(IGoogleAnalyticsReadService googleAnalyticsReadService)
        {
            _googleAnalyticsReadService = googleAnalyticsReadService;
        }

        public ProfilePageViewsDTO GetProfilePageViews(string biobankId, IEnumerable<Data.Entities.OrganisationAnalytic> biobankData)
        {
            var profileData = _googleAnalyticsReadService.FilterByPagePath(biobankData, "/Profile/");
            var summary = _googleAnalyticsReadService.GetSummary(profileData);
            var ranking = _googleAnalyticsReadService.GetRankings(summary);
            (var quarterLabels, var topPageViews) = _googleAnalyticsReadService.GetTopBiobanks(summary, ranking, biobankId, numOfTopBiobanks);
            (var viewsPerQuarter, var viewsAvgs) = _googleAnalyticsReadService.GetQuarterlyAverages(summary, biobankId);
            (var pageRoutes, var routeCount) = _googleAnalyticsReadService.GetPageRoutes(profileData.Where(x => x.OrganisationExternalId == biobankId));

            return new ProfilePageViewsDTO
            {
                QuarterLabels = quarterLabels,
                ProfileQuarters = topPageViews,
                ViewsPerQuarter = viewsPerQuarter,
                ViewsAverages = viewsAvgs,
                PageRouteLabels = pageRoutes,
                RouteCount = routeCount
            };
        }

        public SearchActivityDTO GetSearchActivity(string biobankId, IEnumerable<Data.Entities.OrganisationAnalytic> biobankData)
        {
            var searchData = _googleAnalyticsReadService.FilterByPagePath(biobankData, "/Search/");
            var bbSearchData = searchData.Where(x => x.OrganisationExternalId == biobankId);
            var summary = _googleAnalyticsReadService.GetSummary(searchData);
            var ranking = _googleAnalyticsReadService.GetRankings(summary);
            (var quarterLabels, var topSearches) = _googleAnalyticsReadService.GetTopBiobanks(summary, ranking, biobankId, numOfTopBiobanks);
            (var searchPerQuarter, var searchAvgs) = _googleAnalyticsReadService.GetQuarterlyAverages(summary, biobankId);
            (var searchTypeLabels, var searchTypeCount) = _googleAnalyticsReadService.GetSearchBreakdown(bbSearchData,
                                               _googleAnalyticsReadService.GetSearchType);
            (var searchTermLabels, var searchTermCount) = _googleAnalyticsReadService.GetSearchBreakdown(bbSearchData.Where(x =>
                                               _googleAnalyticsReadService.GetSearchType(x.PagePath) == "Diagnosis"), _googleAnalyticsReadService.GetSearchTerm);
            (var searchFilterLabels, var searchFilterCount) = _googleAnalyticsReadService.GetSearchFilters(bbSearchData);

            return new SearchActivityDTO
            {
                QuarterLabels = quarterLabels,
                SearchQuarters = topSearches,
                SearchPerQuarter = searchPerQuarter,
                SearchAverages = searchAvgs,
                SearchTypeLabels = searchTypeLabels,
                SearchTypeCount = searchTypeCount,
                SearchTermLabels = searchTermLabels,
                SearchTermCount = searchTermCount,
                SearchFilterLabels = searchFilterLabels,
                SearchFilterCount = searchFilterCount
            };
        }

        public ContactRequestsDTO GetContactRequests(string biobankId, IEnumerable<Data.Entities.DirectoryAnalyticEvent> eventData)
        {
            var contactData = _googleAnalyticsReadService.FilterByEvent(eventData, "Add Contact to List");
            var summary = _googleAnalyticsReadService.GetSummary(contactData);
            var ranking = _googleAnalyticsReadService.GetRankings(summary);
            (var quarterLabels, var topContactRequests) = _googleAnalyticsReadService.GetTopBiobanks(summary, ranking, biobankId, numOfTopBiobanks);
            (var requestsPerQuarter, var requestsAvgs) = _googleAnalyticsReadService.GetQuarterlyAverages(summary, biobankId);

            return new ContactRequestsDTO
            {
                QuarterLabels = quarterLabels,
                ContactQuarters = topContactRequests,
                ContactsPerQuarter = requestsPerQuarter,
                ContactAverages = requestsAvgs,
            };
        }

        public async Task<OrganisationAnalyticReportDTO> GetBiobankReport(string biobankId, int year, int quarter, int period)
        {
            var reportRange = _googleAnalyticsReadService.GetRelevantPeriod(year, quarter, period);
            var biobankData = await _googleAnalyticsReadService.GetAllBiobankData(reportRange);
            var eventData = await _googleAnalyticsReadService.GetDirectoryEventData(reportRange);

            //filter by host
            biobankData = _googleAnalyticsReadService.FilterByHost(biobankData);
            eventData = _googleAnalyticsReadService.FilterByHost(eventData);

            //var profileStatus = await GetProfileStatus(biobankId);
            var pageViews = GetProfilePageViews(biobankId, biobankData);
            var searchActivity = GetSearchActivity(biobankId, biobankData);
            var contactRequests = GetContactRequests(biobankId, eventData);


            return new OrganisationAnalyticReportDTO
            {
                ExternalId = biobankId,
                Year = year,
                EndQuarter = quarter,
                ReportPeriod = period,
                NumOfTopBiobanks = numOfTopBiobanks, //maybe get this from api call too
                ProfilePageViews = pageViews,
                SearchActivity = searchActivity,
                ContactRequests = contactRequests,
                Error = new ErrorStatusDTO { ErrorCode = 0, ErrorMessage = "Report Generated Successfully" }
            };
        }
    }
}
