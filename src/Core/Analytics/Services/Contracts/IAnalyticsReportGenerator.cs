using Biobanks.Analytics.Dto;

using System.Threading.Tasks;

namespace Biobanks.Analytics.Services.Contracts
{
    public interface IAnalyticsReportGenerator
    {
        /// <summary>
        /// Generate a report for a specific Organisation from Directory Analytics data
        /// </summary>
        /// <param name="organisationId">Directory External Id for the Organisation</param>
        /// <param name="year"></param>
        /// <param name="endQuarter"></param>
        /// <param name="reportPeriod"></param>
        Task<OrganisationReportDto> GetOrganisationReport(string organisationId, int year, int endQuarter, int reportPeriod);
    }
}