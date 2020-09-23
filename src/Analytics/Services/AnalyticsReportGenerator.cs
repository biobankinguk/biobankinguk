using System.Collections.Generic;
using Analytics.Services.Contracts;
using System.Threading.Tasks;
using Analytics.Services.Dto;
using System.Linq;
using System;

namespace Analytics.Services
{
    public class AnalyticsReportGenerator : IAnalyticsReportGenerator
    {
        private const int numOfTopBiobanks = 10;
        private readonly IGoogleAnalyticsReadService _googleAnalyticsReadService;

        public AnalyticsReportGenerator(IGoogleAnalyticsReadService googleAnalyticsReadService)
        {
            _googleAnalyticsReadService = googleAnalyticsReadService;
        }

        public ProfilePageViewsDto GetProfilePageViews(string biobankId, IEnumerable<Data.Entities.OrganisationAnalytic> biobankData)
        {
            var profileData = _googleAnalyticsReadService.FilterByPagePath(biobankData, "/Profile/");
            var summary = _googleAnalyticsReadService.GetSummary(profileData);
            var ranking = _googleAnalyticsReadService.GetRankings(summary);
            (var quarterLabels, var topPageViews) = _googleAnalyticsReadService.GetTopBiobanks(summary, ranking, biobankId, numOfTopBiobanks);
            (var viewsPerQuarter, var viewsAvgs) = _googleAnalyticsReadService.GetQuarterlyAverages(summary, biobankId);
            (var pageRoutes, var routeCount) = _googleAnalyticsReadService.GetPageRoutes(profileData.Where(x => x.OrganisationExternalId == biobankId));

            return new ProfilePageViewsDto
            {
                QuarterLabels = quarterLabels,
                ProfileQuarters = topPageViews,
                ViewsPerQuarter = viewsPerQuarter,
                ViewsAverages = viewsAvgs,
                PageRouteLabels = pageRoutes,
                RouteCount = routeCount
            };
        }

        public SearchActivityDto GetSearchActivity(string biobankId, IEnumerable<Data.Entities.OrganisationAnalytic> biobankData)
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

            return new SearchActivityDto
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

        public ContactRequestsDto GetContactRequests(string biobankId, IEnumerable<Data.Entities.DirectoryAnalyticEvent> eventData)
        {
            var contactData = _googleAnalyticsReadService.FilterByEvent(eventData, "Add Contact to List");
            var summary = _googleAnalyticsReadService.GetSummary(contactData);
            var ranking = _googleAnalyticsReadService.GetRankings(summary);
            (var quarterLabels, var topContactRequests) = _googleAnalyticsReadService.GetTopBiobanks(summary, ranking, biobankId, numOfTopBiobanks);
            (var requestsPerQuarter, var requestsAvgs) = _googleAnalyticsReadService.GetQuarterlyAverages(summary, biobankId);

            return new ContactRequestsDto
            {
                QuarterLabels = quarterLabels,
                ContactQuarters = topContactRequests,
                ContactsPerQuarter = requestsPerQuarter,
                ContactAverages = requestsAvgs,
            };
        }

        public async Task<OrganisationAnalyticReportDto> GetBiobankReport(string biobankId, int year, int quarter, int period)
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


            return new OrganisationAnalyticReportDto
            {
                ExternalId = biobankId,
                Year = year,
                EndQuarter = quarter,
                ReportPeriod = period,
                NumOfTopBiobanks = numOfTopBiobanks, //maybe get this from api call too
                ProfilePageViews = pageViews,
                SearchActivity = searchActivity,
                ContactRequests = contactRequests,
                Error = new ErrorStatusDto { ErrorCode = 0, ErrorMessage = "Report Generated Successfully" }
            };
        }

        public SessionStatDto GetSessionStats(IEnumerable<Data.Entities.DirectoryAnalyticMetric> metricData)
        {
            var sessionData = _googleAnalyticsReadService.ApplySessionMulitplication(metricData);

            (var sessionNumberLabels, var sessionNumberCount) = _googleAnalyticsReadService.GetSessionNumber(sessionData);
            (var avgBounceRateLabels, var avgBounceRateCount) = _googleAnalyticsReadService.GetWeightedAverage(sessionData, x=>x.BounceRate);
            (var avgNewSessionLabels, var avgNewSessionCount) = _googleAnalyticsReadService.GetWeightedAverage(sessionData, x => x.PercentNewSessions);
            (var avgSessionDurationLabels, var avgSessionDurationVals) = _googleAnalyticsReadService.GetWeightedAverage(sessionData, x => x.AvgSessionDuration);
            var avgSessionDurationCount = avgSessionDurationVals.Select(x => $"{(int)(x / 60)}m {Convert.ToInt32(x % 60)}s").ToList();
            
            return new SessionStatDto {
                SessionNumberLabels = sessionNumberLabels,
                SessionNumberCount = sessionNumberCount,
                AvgBounceRateLabels = avgBounceRateLabels,
                AvgBounceRateCount = avgBounceRateCount,
                AvgNewSessionLabels = avgNewSessionLabels,
                AvgNewSessionCount = avgNewSessionCount,
                AvgSessionDurationLabels = avgSessionDurationLabels,
                AvgSessionDurationCount = avgSessionDurationCount

            };
        }

        public SessionStatDto GetSessionSearchStats(IEnumerable<Data.Entities.DirectoryAnalyticMetric> metricData)
        {
            var searchData = _googleAnalyticsReadService.FilterByPagePath(metricData, "/Search/");
            var sessionData = _googleAnalyticsReadService.ApplySessionMulitplication(searchData);
            
            (var sessionNumberLabels, var sessionNumberCount) = _googleAnalyticsReadService.GetSessionNumber(sessionData);
            (var avgBounceRateLabels, var avgBounceRateCount) = _googleAnalyticsReadService.GetWeightedAverage(sessionData, x => x.BounceRate);
            (var avgNewSessionLabels, var avgNewSessionCount) = _googleAnalyticsReadService.GetWeightedAverage(sessionData, x => x.PercentNewSessions);
            (var avgSessionDurationLabels, var avgSessionDurationVals) = _googleAnalyticsReadService.GetWeightedAverage(sessionData, x => x.AvgSessionDuration);
            var avgSessionDurationCount = avgSessionDurationVals.Select(x => $"{(int)(x / 60)}m {Convert.ToInt32(x % 60)}s").ToList();

            return new SessionStatDto
            {
                SessionNumberLabels = sessionNumberLabels,
                SessionNumberCount = sessionNumberCount,
                AvgBounceRateLabels = avgBounceRateLabels,
                AvgBounceRateCount = avgBounceRateCount,
                AvgNewSessionLabels = avgNewSessionLabels,
                AvgNewSessionCount = avgNewSessionCount,
                AvgSessionDurationLabels = avgSessionDurationLabels,
                AvgSessionDurationCount = avgSessionDurationCount

            };
        }

        public async Task<DirectoryAnalyticReportDto> GetDirectoryReport(int year, int quarter, int period)
        {

            //TODO: REMOVE seed test data
            //await _googleAnalyticsReadService.SeedTestData();

            var reportRange = _googleAnalyticsReadService.GetRelevantPeriod(year, quarter, period);
            var biobankData = await _googleAnalyticsReadService.GetAllBiobankData(reportRange);
            var eventData = await _googleAnalyticsReadService.GetDirectoryEventData(reportRange);
            var metricData = await _googleAnalyticsReadService.GetDirectoryMetricData(reportRange);

            //filter by host
            biobankData = _googleAnalyticsReadService.FilterByHost(biobankData);
            eventData = _googleAnalyticsReadService.FilterByHost(eventData);
            metricData = _googleAnalyticsReadService.FilterByHost(metricData);

            var sessionStats = GetSessionStats(metricData);
            var sessionSearchStats = GetSessionSearchStats(metricData);

            return new DirectoryAnalyticReportDto
            {
                Year = year,
                EndQuarter = quarter,
                ReportPeriod = period,
                NumOfTopBiobanks = numOfTopBiobanks, //maybe get this from api call too

                SessionStats = sessionStats,
                SessionSearchStats = sessionSearchStats,

                Error = new ErrorStatusDto { ErrorCode = 0, ErrorMessage = "Report Generated Successfully" }
            };
        }
    }
}
