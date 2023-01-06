using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Submissions.Api.Areas.Biobank.Models;
using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Services.Directory;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Biobanks.Submissions.Api.Areas.Biobank.Controllers;

public class ReportController : Controller
{
    private readonly AnalyticsReportGenerator _analyticsReportGenerator;
    private readonly IConfigService _configService;
    private readonly IMapper _mapper;
    
    public ReportController(
        AnalyticsReportGenerator analyticsReportGenerator,
        IConfigService configService,
        IMapper mapper
        )
    {
        _analyticsReportGenerator = analyticsReportGenerator;
        _configService = configService;
        _mapper = mapper;
    }

    public async Task<ActionResult> Analytics(int biobankId = 0, int year = 0, int endQuarter = 0, int reportPeriod = 0)
    {
        //If turned off in site config
        if (await _configService.GetFlagConfigValue(ConfigKey.DisplayAnalytics) == false)
            return NotFound();
        
        //set default options
        if (biobankId == 0)
            return RedirectToAction("Index", "Home");
        if (year == 0)
            year = DateTime.Today.Year;
        if (endQuarter == 0)
            endQuarter = ((DateTime.Today.Month + 2) / 3);
        if (reportPeriod == 0)
            reportPeriod = 5;

        try
        {
            var model = _mapper.Map<BiobankAnalyticReport>(await _analyticsReportGenerator.GetBiobankReport(biobankId, year, endQuarter, reportPeriod));
            return View(model);
        }
        catch (Exception e)
        {
            var message = e switch
            {
                JsonSerializationException _ => "The API Response Body could not be processed.",
                KeyNotFoundException _ => "Couldn't find the specified Biobank.",
                HttpRequestException _ => "The API Request failed.",
                _ => "An unknown error occurred and has been logged."
            };

            var outer = new Exception(message, e);

            // Log Error via Application Insights
            var ai = new TelemetryClient();
            ai.TrackException(outer);

            ModelState.AddModelError(String.Empty, message);
            return View(new BiobankAnalyticReport());
        }
    }
}
