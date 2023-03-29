using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Analytics.Services.Contracts;
using Biobanks.Directory.Areas.Admin.Models.Analytics;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Config;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Biobanks.Directory.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]

public class AnalyticsController : Controller
{
    private readonly IConfigService _configService;
    private readonly IMapper _mapper;
    private readonly IDirectoryReportGenerator _directoryReportGenerator;

    public AnalyticsController(
        IConfigService configService,
        IMapper mapper,
        IDirectoryReportGenerator directoryReportGenerator
        )
    {
        _configService = configService;
        _mapper = mapper;
        _directoryReportGenerator = directoryReportGenerator;
    }
    
    public async Task<ActionResult> Index(int year = 0, int endQuarter = 0, int reportPeriod = 0)
    {
        //If turned off in site config
        if (await _configService.GetFlagConfigValue(ConfigKey.DisplayAnalytics) == false)
            return RedirectToAction("LockedRef", "ReferenceData");

        //set default options
        if (year == 0)
            year = DateTime.Today.Year;
        if (endQuarter == 0)
            endQuarter = ((DateTime.Today.Month + 2) / 3);
        if (reportPeriod == 0)
            reportPeriod = 5;

        try
        {
            var model = _mapper.Map<DirectoryAnalyticReport>(await _directoryReportGenerator.GetReport(year, endQuarter, reportPeriod));
            return View(model);
        }
        catch (Exception e)
        {
            var message = e switch
            {
                JsonSerializationException _ => "The API Response Body could not be processed.",
                HttpRequestException _ => "The API Request failed.",
                _ => "An unknown error occurred and has been logged."
            };
          
            ModelState.AddModelError(string.Empty, message);
            return View(new DirectoryAnalyticReport());
        }
    }
}
