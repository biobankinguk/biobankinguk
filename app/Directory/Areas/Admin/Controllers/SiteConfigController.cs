using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Models.Home;
using Biobanks.Submissions.Api.Services.Directory;
using Microsoft.AspNetCore.Mvc;
using Biobanks.Submissions.Api.Areas.Admin.Models;
using Biobanks.Submissions.Api.Areas.Admin.Models.SiteConfig;
using Biobanks.Submissions.Api.Models.Search;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Biobanks.Submissions.Api.Auth;

namespace Biobanks.Submissions.Api.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]

public class SiteConfigController : Controller
{
    
    private readonly IConfigService _configService;
    private readonly ICollectionService _collectionService;
    private readonly IOntologyTermService _ontologyTermService;
  
    public SiteConfigController(
        IConfigService configService,
        ICollectionService collectionService,
        IOntologyTermService ontologyTermService
    )
    {
        _configService = configService;
        _collectionService = collectionService;
        _ontologyTermService = ontologyTermService;
    }
    
    #region Homepage Config
    public async Task<ActionResult> HomepageConfig()
    {
      return View(new HomepageContentModel
      {
          Title = await _configService.GetSiteConfigValue(ConfigKey.HomepageTitle),
          SearchTitle = await _configService.GetSiteConfigValue(ConfigKey.HomepageSearchTitle),
          SearchSubTitle = await _configService.GetSiteConfigValue(ConfigKey.HomepageSearchSubTitle),
          ResourceRegistration = await _configService.GetSiteConfigValue(ConfigKey.HomepageResourceRegistration),
          NetworkRegistration = await _configService.GetSiteConfigValue(ConfigKey.HomepageNetworkRegistration),
          RequireSamplesCollected = await _configService.GetSiteConfigValue(ConfigKey.HomepageSearchRadioSamplesCollected),
          AccessExistingSamples = await _configService.GetSiteConfigValue(ConfigKey.HomepageSearchRadioAccessSamples),
          ResourceRegistrationButton = await _configService.GetSiteConfigValue(ConfigKey.RegisterBiobankTitle),
          NetworkRegistrationButton = await _configService.GetSiteConfigValue(ConfigKey.RegisterNetworkTitle),
      });
    }
    
    [HttpPost]
    public ActionResult HomepageConfig(HomepageContentModel homepage)
      => View(homepage);
    
    [HttpPost]
    public async Task<ActionResult> SaveHomepageConfig(HomepageContentModel homepage)
    {
      await _configService.UpdateSiteConfigsAsync(
          new List<Entities.Data.Config>
          {
              new Entities.Data.Config { Key = ConfigKey.HomepageTitle, Value = homepage.Title ?? "" },
              new Entities.Data.Config { Key = ConfigKey.HomepageSearchTitle, Value = homepage.SearchTitle ?? "" },
              new Entities.Data.Config { Key = ConfigKey.HomepageSearchSubTitle, Value = homepage.SearchSubTitle ?? "" },
              new Entities.Data.Config { Key = ConfigKey.HomepageResourceRegistration, Value = homepage.ResourceRegistration ?? "" },
              new Entities.Data.Config { Key = ConfigKey.HomepageNetworkRegistration, Value = homepage.NetworkRegistration ?? "" },
              new Entities.Data.Config { Key = ConfigKey.HomepageSearchRadioSamplesCollected, Value = homepage.RequireSamplesCollected ?? ""},
              new Entities.Data.Config { Key = ConfigKey.HomepageSearchRadioAccessSamples, Value = homepage.AccessExistingSamples ?? "" },
              new Entities.Data.Config { Key = ConfigKey.RegisterBiobankTitle, Value = homepage.ResourceRegistrationButton ?? "" },
              new Entities.Data.Config { Key = ConfigKey.RegisterNetworkTitle, Value = homepage.NetworkRegistrationButton ?? ""}
          }
      );
    
      // Repopulate current config cache
      await _configService.PopulateSiteConfigCache();
    
      this.SetTemporaryFeedbackMessage("Homepage configuration saved successfully.", FeedbackMessageType.Success);
    
      return Redirect("HomepageConfig");
    }
    
    public ActionResult HomepageConfigPreview()
      => Redirect("HomepageConfig");
    
    [HttpPost]
    public ActionResult HomepageConfigPreview(HomepageContentModel homepage)
      => View("HomepageConfigPreview", homepage);
    
    #endregion
    
    #region Termpage Config
    public async Task<ActionResult> TermpageConfig()
    {
      return View(new TermPageModel
      {
          TermpageContentModel = new TermpageContentModel
          {
              PageInfo = await _configService.GetSiteConfigValue(ConfigKey.TermpageInfo),
          }
      });
    }
    
    [HttpPost]
    public ActionResult TermpageConfig(TermpageContentModel termpage)
    {
      return View(new TermPageModel
      {
          TermpageContentModel = termpage
      });
    }
    
    
    [HttpPost]
    public async Task<ActionResult> SaveTermpageConfig(TermpageContentModel termpage)
    {
      await _configService.UpdateSiteConfigsAsync(
          new List<Entities.Data.Config>
          {
              new Entities.Data.Config { Key = ConfigKey.TermpageInfo, Value = termpage.PageInfo ?? "" }
          }
      );
    
      // Repopulate current config cache
      await _configService.PopulateSiteConfigCache();
    
      this.SetTemporaryFeedbackMessage("Term page configuration saved successfully.", FeedbackMessageType.Success);
    
      return Redirect("TermpageConfig");
    }
    
    public ActionResult TermpageConfigPreview()
      => Redirect("TermpageConfig");
    
    [HttpPost]
    public async Task<ActionResult> TermpageConfigPreview(TermpageContentModel termpage)
    {
      // Populate Snomed Terms for Preview View
      var collections = await _collectionService.List();
    
      var ontologyTerms = collections
          .Where(x => x.SampleSets.Any())
          .Select(x => x.OntologyTerm)
          .Distinct();
    
      // Find CollectionCapabilityCount For Each OntologyTerm
      var ontologyTermModels = ontologyTerms.Select(x =>
    
          Task.Run(async () => new ReadOntologyTermModel
          {
              OntologyTermId = x.Id,
              Description = x.Value,
              CollectionCapabilityCount = await _ontologyTermService.CountCollectionCapabilityUsage(x.Id),
              OtherTerms = x.OtherTerms
          })
          .Result
      );
    
      return View("TermpageConfigPreview", new TermPageModel
      {
          OntologyTermsModel = ontologyTermModels,
          TermpageContentModel = termpage
      });
    }
    
    
    #endregion
    
    #region Register Biobank and Network Pages Config
    public async Task<ActionResult> RegisterPagesConfig()
    {
        var model = new RegisterConfigModel();
        model.BiobankTitle = await _configService.GetSiteConfigValue(ConfigKey.RegisterBiobankTitle);
        model.BiobankDescription = await _configService.GetSiteConfigValue(ConfigKey.RegisterBiobankDescription);
        model.NetworkTitle = await _configService.GetSiteConfigValue(ConfigKey.RegisterNetworkTitle);
        model.NetworkDescription = await _configService.GetSiteConfigValue(ConfigKey.RegisterNetworkDescription);
        model.EnableRegistrationHelpUrl = await _configService.GetSiteConfigValue(ConfigKey.EnableRegisterRegistrationHelpUrl);
        model.RegistrationHelpUrl = await _configService.GetSiteConfigValue(ConfigKey.RegisterRegistrationHelpUrl);
        model.RegistrationEmails = await _configService.GetSiteConfigValue(ConfigKey.RegistrationEmails);
        return View(model);
    }
    
    
    [HttpPost]
    public async Task<ActionResult> SaveRegisterPagesConfig(RegisterConfigModel registerConfigModel)
    {
      await _configService.UpdateSiteConfigsAsync(
          new List<Entities.Data.Config>
          {
              new Entities.Data.Config { Key = ConfigKey.RegisterBiobankTitle, Value = registerConfigModel.BiobankTitle ?? ""},
              new Entities.Data.Config { Key = ConfigKey.RegisterBiobankDescription, Value = registerConfigModel.BiobankDescription ?? "" },
              new Entities.Data.Config { Key = ConfigKey.RegisterNetworkTitle, Value = registerConfigModel.NetworkTitle ?? ""},
              new Entities.Data.Config { Key = ConfigKey.RegisterNetworkDescription, Value = registerConfigModel.NetworkDescription ?? "" },
              new Entities.Data.Config { Key = ConfigKey.EnableRegisterRegistrationHelpUrl, Value = registerConfigModel.EnableRegistrationHelpUrl ?? "" },
              new Entities.Data.Config { Key = ConfigKey.RegisterRegistrationHelpUrl, Value = registerConfigModel.RegistrationHelpUrl ?? "" },
              new Entities.Data.Config { Key = ConfigKey.RegistrationEmails, Value = registerConfigModel.RegistrationEmails ?? "" }
          }
      );
    
      // Repopulate current config cache
      await _configService.PopulateSiteConfigCache();
      
      this.SetTemporaryFeedbackMessage("Register configuration saved successfully.", FeedbackMessageType.Success);
      return Redirect("RegisterPagesConfig");
    }
    #endregion
    
    
    #region Site Config
    public async Task<ActionResult> SiteConfig()
    {
      return View((await _configService.ListSiteConfigsAsync("site.display"))
          .Select(x => new SiteConfigModel
          {
              Key = x.Key,
              Value = x.Value,
              Name = x.Name,
              Description = x.Description,
              ReadOnly = x.ReadOnly
          }));
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateSiteConfig(IEnumerable<SiteConfigModel> values)
    {
      // Update Database Config
      await _configService.UpdateSiteConfigsAsync(
          values
              .OrderBy(x => x.Key)
              .Select(x => new Entities.Data.Config
              {
                  Key = x.Key,
                  Value = x.Value ?? "", // Store nulls as empty strings
              })
          );
    
      // Repopulate current config cache
      await _configService.PopulateSiteConfigCache();
    
      return Ok(new
      {
          success = true,
          redirect = "UpdateSiteConfigSuccess"
      });
    }
    
    public ActionResult UpdateSiteConfigSuccess()
    {
      this.SetTemporaryFeedbackMessage("Site configuration saved successfully.", FeedbackMessageType.Success);
      return RedirectToAction("SiteConfig");
    }
    #endregion
    
    #region Sample Resource Config
    public async Task<ActionResult> SampleResourceConfig()
    {
      return View((await _configService.ListSiteConfigsAsync("site.sampleresource"))
          .Select(x => new SiteConfigModel
          {
              Key = x.Key,
              Value = x.Value,
              Name = x.Name,
              Description = x.Description,
              ReadOnly = x.ReadOnly
          }));
    }
    
    public ActionResult SampleResourceConfigSuccess()
    {
      this.SetTemporaryFeedbackMessage("Configuration saved successfully.", FeedbackMessageType.Success);
      return RedirectToAction("SampleResourceConfig");
    }
    
    #endregion
    
    //Method for updating specific Reference Terms Names via Config
    public async Task<IActionResult> UpdateReferenceTermName(string newReferenceTermKey, string newReferenceTermName)
    {
    
      List<Entities.Data.Config> values = new List<Entities.Data.Config>();
    
      values.Add(new Entities.Data.Config
      {
          Key = newReferenceTermKey,
          Value = newReferenceTermName ?? ""
      });
    
    
      // Update Database Config
      await _configService.UpdateSiteConfigsAsync(values);
    
      // Repopulate current config cache
      await _configService.PopulateSiteConfigCache();
      
      this.SetTemporaryFeedbackMessage("The Reference Term has been overriden successfully.", FeedbackMessageType.Success);
      return Ok(new
      {
          success = true,
      });
    }
    
}
