using Biobanks.Analytics.Services.Contracts;
using Biobanks.Submissions.Api.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = nameof(AuthPolicies.IsBasicAuthenticated))]
    public class AnalyticsController : ControllerBase
    {

        private readonly IAnalyticsReportGenerator _analyticsReportGenerator;
        
        /// <inheritdoc />
        public AnalyticsController(IAnalyticsReportGenerator analyticsReportGenerator)
        {
            _analyticsReportGenerator = analyticsReportGenerator;
        }

        //"GetAnalyticsReport/{biobankId}/{year}/{endQuarter}/{reportPeriod}"
        /// <summary>
        /// </summary>
        /// <param name="organisationId">The ID of the biobank to operate on.</param>
        /// <param name="year">The ID of the biobank to operate on.</param>
        /// <param name="endQuarter">The ID of the biobank to operate on.</param>
        /// <param name="reportPeriod">The ID of the biobank to operate on.</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200)]
        public async Task GetAnalyticsReportAsync(string organisationId, int year, int endQuarter, int reportPeriod)
            => Ok(await _analyticsReportGenerator.GetBiobankReport(organisationId, year, endQuarter, reportPeriod));

        //GetDirectoryAnalyticsReport/{year}/{endQuarter}/{reportPeriod}
        /// <summary>
        /// </summary>
        /// <param name="year">The ID of the biobank to operate on.</param>
        /// <param name="endQuarter">The ID of the biobank to operate on.</param>
        /// <param name="reportPeriod">The ID of the biobank to operate on.</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200)]
        public async Task GetDirectoryAnalyticsReport(int year, int endQuarter, int reportPeriod)
            => Ok(await _analyticsReportGenerator.GetDirectoryReport(year, endQuarter, reportPeriod));

    }
}
