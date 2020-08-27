using Analytics.Data.Entities;
using Analytics.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analytics.Services.Contracts
{
    public interface IAnalyticsReportGenerator
    {
        Task<OrganisationAnalyticReportDTO> GetBiobankReport(string biobankId, int year, int quarter, int period);
        ContactRequestsDTO GetContactRequests(string biobankId, IEnumerable<DirectoryAnalyticEvent> eventData);
        ProfilePageViewsDTO GetProfilePageViews(string biobankId, IEnumerable<OrganisationAnalytic> biobankData);
        SearchActivityDTO GetSearchActivity(string biobankId, IEnumerable<OrganisationAnalytic> biobankData);
    }
}