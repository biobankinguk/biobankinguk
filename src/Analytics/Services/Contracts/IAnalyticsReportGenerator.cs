using Analytics.Data.Entities;
using Analytics.Services.Dto;
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
    }
}