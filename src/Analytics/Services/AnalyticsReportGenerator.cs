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
        private readonly int _numOfTopBiobanks; //default: 10
        private readonly int _eventThreshold; //default: 30
        private readonly bool _filterByHost; //default: true
        private readonly string _hostname;

        private readonly IGoogleAnalyticsReadService _googleAnalyticsReadService;

        public AnalyticsReportGenerator(IGoogleAnalyticsReadService googleAnalyticsReadService)
        {
            _googleAnalyticsReadService = googleAnalyticsReadService;
            _numOfTopBiobanks = Convert.ToInt32(Environment.GetEnvironmentVariable("metric-threshold"));
            _eventThreshold = Convert.ToInt32(Environment.GetEnvironmentVariable("event-threshold"));
            _filterByHost = Convert.ToBoolean(Environment.GetEnvironmentVariable("filterby-host"));
            _hostname = Environment.GetEnvironmentVariable("directory-hostname");
        }

        public ProfilePageViewsDto GetProfilePageViews(string biobankId, IEnumerable<Data.Entities.OrganisationAnalytic> biobankData)
        {
            var profileData = _googleAnalyticsReadService.FilterByPagePath(biobankData, "/Profile/");
            var summary = _googleAnalyticsReadService.GetSummary(profileData);
            var ranking = _googleAnalyticsReadService.GetRankings(summary);
            (var quarterLabels, var topPageViews) = _googleAnalyticsReadService.GetTopBiobanks(summary, ranking, biobankId, _numOfTopBiobanks);
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
            (var quarterLabels, var topSearches) = _googleAnalyticsReadService.GetTopBiobanks(summary, ranking, biobankId, _numOfTopBiobanks);
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
            (var quarterLabels, var topContactRequests) = _googleAnalyticsReadService.GetTopBiobanks(summary, ranking, biobankId, _numOfTopBiobanks);
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
            if (_filterByHost == true && !String.IsNullOrEmpty(_hostname))
            {
                biobankData = _googleAnalyticsReadService.FilterByHost(biobankData,_hostname);
                eventData = _googleAnalyticsReadService.FilterByHost(eventData, _hostname);
            }

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
                NumOfTopBiobanks = _numOfTopBiobanks, //maybe get this from api call too
                ProfilePageViews = pageViews,
                SearchActivity = searchActivity,
                ContactRequests = contactRequests,
                Error = new ErrorStatusDto { ErrorCode = 0, ErrorMessage = "Report Generated Successfully" }
            };
        }

        public SessionStatDto GetSessionStats(IEnumerable<Data.Entities.DirectoryAnalyticMetric> metricData)
        {
            var sessionData = _googleAnalyticsReadService.ApplySessionMulitplication(metricData);

            (var sessionNumberLabels, var sessionNumberCount) = _googleAnalyticsReadService.GetSessionCount(sessionData);
            (var avgBounceRateLabels, var avgBounceRateCount) = _googleAnalyticsReadService.GetWeightedAverage(sessionData, x=>x.BounceRate);
            (var avgNewSessionLabels, var avgNewSessionCount) = _googleAnalyticsReadService.GetWeightedAverage(sessionData, x => x.PercentNewSessions);
            (var avgSessionDurationLabels, var avgSessionDurationCount) = _googleAnalyticsReadService.GetWeightedAverage(sessionData, x => x.AvgSessionDuration);
           // var avgSessionDurationCountLabel = avgSessionDurationCount.Select(x => $"{(int)(x / 60)}m {Convert.ToInt32(x % 60)}s").ToList(); //do in frontend
            
            return new SessionStatDto {
                SessionNumberLabels = sessionNumberLabels,
                SessionNumberCount = sessionNumberCount,
                AvgBounceRateLabels = avgBounceRateLabels,
                AvgBounceRateCount = avgBounceRateCount,
                AvgNewSessionLabels = avgNewSessionLabels,
                AvgNewSessionCount = avgNewSessionCount,
                AvgSessionDurationLabels = avgSessionDurationLabels,
                AvgSessionDurationCount = avgSessionDurationCount,

            };
        }

        public SessionStatDto GetSessionSearchStats(IEnumerable<Data.Entities.DirectoryAnalyticMetric> metricData)
        {
            var searchData = _googleAnalyticsReadService.FilterByPagePath(metricData, "/Search/");
            var sessionData = _googleAnalyticsReadService.ApplySessionMulitplication(searchData);

            var sessionSummary = _googleAnalyticsReadService.GetSummary(sessionData, x => x.Sessions);
            (var sessionNumberLabels, var sessionNumberCount) = _googleAnalyticsReadService.GetSessionCount(sessionData);
            (var avgBounceRateLabels, var avgBounceRateCount) = _googleAnalyticsReadService.GetWeightedAverage(sessionData, x => x.BounceRate);
            (var avgNewSessionLabels, var avgNewSessionCount) = _googleAnalyticsReadService.GetWeightedAverage(sessionData, x => x.PercentNewSessions);
            (var avgSessionDurationLabels, var avgSessionDurationCount) = _googleAnalyticsReadService.GetWeightedAverage(sessionData, x => x.AvgSessionDuration);

            return new SessionStatDto
            {
                SessionNumberLabels = sessionNumberLabels,
                SessionNumberCount = sessionNumberCount,
                AvgBounceRateLabels = avgBounceRateLabels,
                AvgBounceRateCount = avgBounceRateCount,
                AvgNewSessionLabels = avgNewSessionLabels,
                AvgNewSessionCount = avgNewSessionCount,
                AvgSessionDurationLabels = avgSessionDurationLabels,
                AvgSessionDurationCount = avgSessionDurationCount,
            };
        }

        public SearchCharacteristicDto GetSearchCharacteristics(IEnumerable<Data.Entities.OrganisationAnalytic> biobankData)
        {
            var searchData = _googleAnalyticsReadService.FilterByPagePath(biobankData, "/Search/");

            (var searchTypeLabels, var searchTypeCount) = _googleAnalyticsReadService.GetSearchBreakdown(searchData,
                                               _googleAnalyticsReadService.GetSearchType);
            (var searchTermLabels, var searchTermCount) = _googleAnalyticsReadService.GetSearchBreakdown(searchData.Where(x =>
                                               _googleAnalyticsReadService.GetSearchType(x.PagePath) == "Diagnosis"), _googleAnalyticsReadService.GetSearchTerm);
            (var searchFilterLabels, var searchFilterCount) = _googleAnalyticsReadService.GetSearchFilters(searchData);

            return new SearchCharacteristicDto {
                SearchTypeLabels = searchTypeLabels,
                SearchTypeCount = searchTypeCount,
                SearchTermLabels = searchTermLabels,
                SearchTermCount = searchTermCount,
                SearchFilterLabels = searchFilterLabels,
                SearchFilterCount = searchFilterCount
            };
        }

        public EventStatDto GetEventStats(IEnumerable<Data.Entities.DirectoryAnalyticEvent> eventData)
        {
            var contactData = _googleAnalyticsReadService.FilterByEvent(eventData, "Add Contact to List");
            var mailtoData = _googleAnalyticsReadService.FilterByEvent(eventData, "Mailto clicked");
            (var contactNumberLabels, var contactNumberCount) = _googleAnalyticsReadService.GetContactCount(contactData);
            (var filteredContactLabels, var filteredContactCount) = _googleAnalyticsReadService.GetFilteredEventCount(contactData, _eventThreshold);
            (var filteredMailtoLabels, var filteredMailtoCount) = _googleAnalyticsReadService.GetFilteredEventCount(mailtoData, _eventThreshold);

            return new EventStatDto { 
                ContactNumberLabels = contactNumberLabels,
                ContactNumberCount = contactNumberCount,
                FilteredContactLabels = filteredContactLabels,
                FilteredContactCount = filteredContactCount,
                FilteredMailToLabels = filteredMailtoLabels,
                FilteredMailToCount = filteredMailtoCount
            };
        }

        public ProfilePageStatDto GetProfilePageStats(IEnumerable<Data.Entities.OrganisationAnalytic> biobankData)
        {
            var profileData = _googleAnalyticsReadService.FilterByPagePath(biobankData, "/Profile/");

            var profileSources = _googleAnalyticsReadService.GetPageSources(profileData, _numOfTopBiobanks);
            (var pageRoutes, var routeCount) = _googleAnalyticsReadService.GetPageRoutes(profileData);
            

            return new ProfilePageStatDto
            {
                ProfileSources = profileSources,
                PageRouteLabels = pageRoutes,
                RouteCount = routeCount
            };
        }


        public async Task<DirectoryAnalyticReportDto> GetDirectoryReport(int year, int quarter, int period)
        {
            var reportRange = _googleAnalyticsReadService.GetRelevantPeriod(year, quarter, period);
            var biobankData = await _googleAnalyticsReadService.GetAllBiobankData(reportRange);
            var eventData = await _googleAnalyticsReadService.GetDirectoryEventData(reportRange);
            var metricData = await _googleAnalyticsReadService.GetDirectoryMetricData(reportRange);

            //filter by host
            if (_filterByHost == true && !String.IsNullOrEmpty(_hostname))
            {
                biobankData = _googleAnalyticsReadService.FilterByHost(biobankData, _hostname);
                eventData = _googleAnalyticsReadService.FilterByHost(eventData, _hostname);
                metricData = _googleAnalyticsReadService.FilterByHost(metricData, _hostname);
            }

            var sessionStats = GetSessionStats(metricData);
            var sessionSearchStats = GetSessionSearchStats(metricData);
            var searchCharacteristics = GetSearchCharacteristics(biobankData);
            var eventStats = GetEventStats(eventData);
            var profilePageStats = GetProfilePageStats(biobankData);

            return new DirectoryAnalyticReportDto
            {
                Year = year,
                EndQuarter = quarter,
                ReportPeriod = period,
                NumOfTopBiobanks = _numOfTopBiobanks, //maybe get this from api call instead of config/env vars?
                EventsPerCityThreshold = _eventThreshold, //maybe get this from api call instead of config/env vars?

                SessionStats = sessionStats,
                SessionSearchStats = sessionSearchStats,
                SearchCharacteristics = searchCharacteristics,
                EventStats = eventStats,
                ProfilePageStats = profilePageStats,

                Error = new ErrorStatusDto { ErrorCode = 0, ErrorMessage = "Report Generated Successfully" }
            };
        }
    }
}
