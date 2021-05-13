using Biobanks.Analytics.Dto;
using Biobanks.Analytics.Services.Contracts;
using Biobanks.Submissions.Api.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(nameof(AuthPolicies.IsSuperAdmin))]
    public class AnalyticsController : ControllerBase
    {
        private readonly IDirectoryReportGenerator _directoryReports;
        private readonly IOrganisationReportGenerator _organisationReports;

        public AnalyticsController(
            IDirectoryReportGenerator directoryReports,
            IOrganisationReportGenerator organisationReports)
        {
            _directoryReports = directoryReports;
            _organisationReports = organisationReports;
        }

        // TODO: Swagger
        [HttpGet("{year}/{endQuarter}/{reportPeriod}/{organisationId}")]
        public async Task<ActionResult<OrganisationReportDto>> OrganisationReport(
            int year,
            int endQuarter,
            int reportPeriod,
            string organisationId)
            => await _organisationReports.GetReport(organisationId, year, endQuarter, reportPeriod);

        // TODO: Swagger
        [HttpGet("{year}/{endQuarter}/{reportPeriod}")]
        public async Task<ActionResult<DirectoryReportDto>> DirectoryReport(
            int year,
            int endQuarter,
            int reportPeriod)
            => await _directoryReports.GetReport(year, endQuarter, reportPeriod);
    }
}
