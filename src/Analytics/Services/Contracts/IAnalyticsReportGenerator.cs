using Analytics.Services.Dto;
using Biobanks.Entities.Data.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analytics.Services.Contracts
{
    public interface IAnalyticsReportGenerator
    {
        Task<OrganisationAnalyticReportDto> GetBiobankReport(string biobankId, int year, int quarter, int period);
        ContactRequestsDto GetContactRequests(string biobankId, IEnumerable<DirectoryAnalyticEvent> eventData);
        ProfilePageViewsDto GetProfilePageViews(string biobankId, IEnumerable<OrganisationAnalytic> biobankData);
        SearchActivityDto GetSearchActivity(string biobankId, IEnumerable<OrganisationAnalytic> biobankData);

        Task<DirectoryAnalyticReportDto> GetDirectoryReport(int year, int quarter, int period);
    }
}