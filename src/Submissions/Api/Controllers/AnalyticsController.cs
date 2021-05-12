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
    //[Authorize(nameof(AuthPolicies.IsSuperAdmin))]
    [AllowAnonymous]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsReportGenerator _reports;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            IAnalyticsReportGenerator reports,
            ILogger<AnalyticsController> logger)
        {
            _reports = reports;
            _logger = logger;
        }

        // TODO: Swagger
        [HttpGet("{year}/{endQuarter}/{reportPeriod}/{organisationId}")]
        public async Task<ActionResult<OrganisationReportDto>> OrganisationReport(
            int year,
            int endQuarter,
            int reportPeriod,
            string organisationId)
            => await _reports.GetOrganisationReport(organisationId, year, endQuarter, reportPeriod);

        // TODO: Swagger
        //[HttpGet("{year}/{endQuarter}/{reportPeriod}")]
        //public async Task<ActionResult<OrganisationReportDto>> DirectoryReport(
        //    int year,
        //    int endQuarter,
        //    int reportPeriod)
        //    => await _reports.GetDirectoryReport(year, endQuarter, reportPeriod);
    }
}
