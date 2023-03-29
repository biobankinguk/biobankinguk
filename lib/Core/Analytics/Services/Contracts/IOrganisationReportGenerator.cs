using Biobanks.Analytics.Dto;

using System.Threading.Tasks;

namespace Biobanks.Analytics.Services.Contracts
{
    public interface IOrganisationReportGenerator
    {
        /// <summary>
        /// Generate a report for a specific Organisation from Directory Analytics data
        /// </summary>
        /// <param name="organisationId">Directory External Id for the Organisation</param>
        /// <param name="year">The calendar year in which `quarter` is</param>
        /// <param name="quarter">The quarter of `year` to END the reporting period at</param>
        /// <param name="period">The length in months of the reporting period, working back from the end of `quarter` in `year`</param>
        Task<OrganisationReportDto> GetReport(string organisationId, int year, int quarter, int period);
    }
}