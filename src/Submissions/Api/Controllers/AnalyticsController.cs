using Biobanks.Analytics.Dto;
using Biobanks.Analytics.Services.Contracts;
using Biobanks.Submissions.Api.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers
{
    /// <summary>
    /// Controller for generating Analytics Report data
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize(nameof(AuthPolicies.IsSuperAdmin))]
    public class AnalyticsController : ControllerBase
    {
        private readonly IDirectoryReportGenerator _directoryReports;
        private readonly IOrganisationReportGenerator _organisationReports;

        /// <inheritdoc />
        public AnalyticsController(
            IDirectoryReportGenerator directoryReports,
            IOrganisationReportGenerator organisationReports)
        {
            _directoryReports = directoryReports;
            _organisationReports = organisationReports;
        }

        /// <summary>
        /// Generate a report from the currently stored Directory Analytics data, for a specific Organisation
        /// </summary>
        /// <param name="year">Year in which reporting period ends</param>
        /// <param name="endQuarter">Quarter in which reporting period ends</param>
        /// <param name="reportPeriod">Length in months of reporting period, working back from the end of the specified quarter</param>
        /// <param name="organisationId">External Id of the Directory Organisation to generate a report for</param>
        [HttpGet("{year}/{endQuarter}/{reportPeriod}/{organisationId}")]
        [SwaggerResponse(200)]
        public async Task<ActionResult<OrganisationReportDto>> OrganisationReport(
            int year,
            int endQuarter,
            int reportPeriod,
            string organisationId)
            => await _organisationReports.GetReport(organisationId, year, endQuarter, reportPeriod);

        /// <summary>
        /// Generate an overall Directory report from the currently stored Directory Analytics data
        /// </summary>
        /// <param name="year">Year in which reporting period ends</param>
        /// <param name="endQuarter">Quarter in which reporting period ends</param>
        /// <param name="reportPeriod">Length in months of reporting period, working back from the end of the specified quarter</param>
        [HttpGet("{year}/{endQuarter}/{reportPeriod}")]
        [SwaggerResponse(200)]
        public async Task<ActionResult<DirectoryReportDto>> DirectoryReport(
            int year,
            int endQuarter,
            int reportPeriod)
            => await _directoryReports.GetReport(year, endQuarter, reportPeriod);
    }
}
