using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Data.Transforms.Url;
using Biobanks.Directory.Areas.Network.Models;
using Biobanks.Directory.Areas.Network.Models.Profile;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Config;
using Biobanks.Directory.Constants;
using Biobanks.Directory.Extensions;
using Biobanks.Directory.Models.Emails;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.Directory.Dto;
using Biobanks.Directory.Services.EmailServices.Contracts;
using Biobanks.Directory.Services.Submissions;
using Biobanks.Directory.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Biobanks.Directory.Areas.Network.Controllers;

[Area("Network")]
[Authorize(nameof(AuthPolicies.IsNetworkAdmin))]

public class ProfileController : Controller
{
  private readonly INetworkService _networkService;
  private readonly ILogoStorageProvider _logoStorageProvider;
  private readonly IReferenceDataCrudService<SopStatus> _sopStatusService;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IEmailService _emailService;
  private readonly IConfigService _configService;
  private readonly IOrganisationDirectoryService _organisationService;
  private readonly IBiobankService _biobankService;
  private readonly SignInManager<ApplicationUser> _signInManager;

  public ProfileController(INetworkService networkService, ILogoStorageProvider logoStorageProvider, 
    IReferenceDataCrudService<SopStatus> sopStatusService, 
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IConfigService configService,
    IOrganisationDirectoryService organisationService,
    IBiobankService biobankService,
    SignInManager<ApplicationUser> signInManager
    )
  {
    _networkService = networkService;
    _logoStorageProvider = logoStorageProvider;
    _sopStatusService = sopStatusService;
    _userManager = userManager;
    _emailService = emailService;
    _configService = configService;
    _organisationService = organisationService;
    _biobankService = biobankService;
    _signInManager = signInManager;
  }

  private async Task<List<RegisterEntityAdminModel>> GetAdminsAsync(int networkId, bool excludeCurrentUser)
  {
    //we exclude the current user when we are making the list for them
    //but we may want the full list in other circumstances
    var admins =
        (await _networkService.ListAdmins(networkId))
            .Select(nwAdmin => new RegisterEntityAdminModel
            {
              UserId = nwAdmin.Id,
              UserFullName = nwAdmin.Name,
              UserEmail = nwAdmin.Email,
              EmailConfirmed = nwAdmin.EmailConfirmed
            }).ToList();

    if (excludeCurrentUser)
    {
      admins.Remove(admins.FirstOrDefault(x => x.UserId == _userManager.GetUserId(User)));
    }

    return admins;
  }
  
  [Authorize(nameof(AuthPolicies.HasNetworkClaim))]
  public async Task<ActionResult> Details(int networkId)
  {
    return View(await GetNetworkDetailsModelAsync(networkId));
  }

  /// <summary>
  /// Route to create a new network.
  /// </summary>
  /// <param name="networkId">This is actually the request ID,
  /// but is called this for ease of routing.</param>
  /// <returns>Edit view for network.</returns>
  [Authorize(nameof(AuthPolicies.HasNetworkRequestClaim))]
  public async Task<ActionResult> Create(int networkId)
  {
    // Check the organisation request has not been accepted
    var request = await _networkService.GetRegistrationRequest(networkId);
    if (request.NetworkCreatedDate.HasValue)
    {
      // get the biobank to redirect to.
      var network = await _organisationService.GetByName(request.NetworkName);
      this.SetTemporaryFeedbackMessage("This biobank has already been created.", FeedbackMessageType.Info);
      RedirectToAction("Biobanks", "Profile", new { area = "Network", network.OrganisationId });
    }
      
    var sampleResource = await _configService.GetSiteConfigValue(ConfigKey.SampleResourceName);
    this.SetTemporaryFeedbackMessage("Please fill in the details below for your " + sampleResource + ". Once you have completed these, you'll be able to perform other administration tasks",
      FeedbackMessageType.Info);
      
    var model = await NewNetworkDetailsModelAsync(networkId);
          
    // Reset the biobankId in the model state so the form does not populate it.
    // Ensures a new biobank is created.
    ModelState.SetModelValue("biobankId", new ValueProviderResult());
    return View("Edit", model);
  }

  [Authorize(nameof(AuthPolicies.HasNetworkClaim))]
  public async Task<ActionResult> Edit(int networkId, bool detailsIncomplete = false)
  {
    if (detailsIncomplete)
      this.SetTemporaryFeedbackMessage("Please fill in the details below for your network. Once you have completed these, you'll be able to perform other administration tasks",
          FeedbackMessageType.Info);
    
    //network id means we're dealing with an existing network
    return View(await GetNetworkDetailsModelAsync(networkId)); 
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<ActionResult> Edit(NetworkDetailsModel model)
  {
    //Populate the SOP Statuses again, as they're not posted
    model.SopStatuses = await GetSopStatusKeyValuePairsAsync();

    if (!ModelState.IsValid)
    {
      return View(model);
    }

    //Extra form validation that the model state can't do for us

    //Logo, if any
    try
    {
      model.Logo.ValidateAsLogo();
    }
    catch (BadImageFormatException ex)
    {
      ModelState.AddModelError("", ex.Message);
      return View(model);
    }

    //URL, if any
    try
    {
      model.Url = UrlTransformer.Transform(model.Url);
    }
    catch (InvalidUrlSchemeException e)
    {
      ModelState.AddModelError("", e.Message);
      return View(model);
    }
    catch (UriFormatException)
    {
      ModelState.AddModelError("",
          $"{model.Url} is not a valid URL.");
      return View(model);
    }
    if (!string.IsNullOrWhiteSpace(model.Url))
    {
      try
      {
        // Attempt to access the page and see if it returns 2xx code
        // Automatically follows redirects (though the RFC says .NET Framework is being naughty by doing this!)
        var response = await new HttpClient().GetAsync(model.Url);
        if (!response.IsSuccessStatusCode)
        {
          ModelState.AddModelError(string.Empty, $"{model.Url} does not appear to be a valid URL.");
          return View(model);
        }
      }
      catch
      {
        ModelState.AddModelError(string.Empty, $"Could not access URL {model.Url}.");
        return View(model);
      }
    }

    var create = model.NetworkId == null;
    var logoName = model.LogoName;

    // if updating, try and upload a logo now (ensuring logoName is correct)
    if (!create && model.RemoveLogo)
    {
      logoName = "";
      await _logoStorageProvider.RemoveLogo(model.NetworkId.Value);
    }
    else if (!create && model.Logo != null)
    {
      try
      {
        logoName = await UploadNetworkLogoAsync(model.Logo, model.NetworkId);
      }
      catch (ArgumentNullException)
      {
      } //no problem, just means no logo uploaded in this form submission
    }


    var networkDto = new NetworkDTO
    {
      Name = model.NetworkName,
      Description = model.Description,
      Url = model.Url,
      Email = model.ContactEmail,
      Logo = logoName,
      SopStatusId = model.SopStatus
    };

    if (create)
    {
      var network = await _networkService.Create(networkDto);
      var currentUser = await _userManager.GetUserAsync(HttpContext.User);
      await _networkService.AddNetworkUser(currentUser.Id, network.NetworkId);

      //update the request to show network created
      var request = await _networkService.GetRegistrationRequestByEmail(User.Identity.Name);
      request.NetworkCreatedDate = DateTime.Now;
      await _networkService.UpdateRegistrationRequest(request);

      //add a claim now that they're associated with the network
      await _userManager.AddClaimsAsync(currentUser, new List<Claim>
                {
                    new Claim(CustomClaimType.Network, JsonConvert.SerializeObject(new KeyValuePair<int, string>(network.NetworkId, networkDto.Name)))
                });
      
      // Resign in the user so their claims are repopulated.
      await _signInManager.RefreshSignInAsync(currentUser);

      //Logo upload (now we have the id, we can form the filename)
      if (model.Logo != null)
      {
        try
        {
          networkDto.Logo = await UploadNetworkLogoAsync(model.Logo, network.NetworkId);
          await _networkService.Update(networkDto);
        }
        catch (ArgumentNullException)
        {
        } //no problem, just means no logo uploaded in this form submission
      }
    }
    else
    {
      //add Update only bits of the Entity
      networkDto.NetworkId = model.NetworkId.Value;

      // update handover Url components
      networkDto.ContactHandoverEnabled = model.ContactHandoverEnabled;
      networkDto.HandoverBaseUrl = model.HandoverBaseUrl;
      networkDto.HandoverOrgIdsUrlParamName = model.HandoverOrgIdsUrlParamName;
      networkDto.MultipleHandoverOrdIdsParams = model.MultipleHandoverOrdIdsParams;
      networkDto.HandoverNonMembers = model.HandoverNonMembers;
      networkDto.HandoverNonMembersUrlParamName = model.HandoverNonMembersUrlParamName;

      await _networkService.Update(networkDto);
    }

    this.SetTemporaryFeedbackMessage("Network details updated!", FeedbackMessageType.Success);

    //Back to the profile to view your saved changes
    return RedirectToAction("Details", new { networkId = networkDto.NetworkId });
  }

  private async Task<string> UploadNetworkLogoAsync(IFormFile logo, int? networkId)
  {
    const string networkLogoPrefix = "NETWORK-";

    var logoStream = logo.ToProcessableStream();
    await using var memoryStream = new MemoryStream();
    await logoStream.CopyToAsync(memoryStream);

    var logoName =
        await
            _logoStorageProvider.StoreLogo(
                (System.IO.MemoryStream)memoryStream,
                logo.FileName,
                logo.ContentType,
                networkLogoPrefix + networkId);
    return logoName;
  }

  private async Task<List<KeyValuePair<int, string>>> GetSopStatusKeyValuePairsAsync()
  {
    var sopStatuses = await _sopStatusService.List();

    return sopStatuses
        .Select(status => new KeyValuePair<int, string>(status.Id, status.Value))
        .ToList();
  }
  
  private async Task<NetworkDetailsModel> NewNetworkDetailsModelAsync(int networkId)
  {
    //prep the SOP Statuses as KeyValuePair for the model
    var sopStatuses = await GetSopStatusKeyValuePairsAsync();

    //Network doesn't exist yet, but the request does, so get the name
    var request = await _networkService.GetRegistrationRequest(networkId);

    //validate that the request is accepted
    if (request.AcceptedDate == null) return null;

    return new NetworkDetailsModel
    {
      NetworkName = request.NetworkName,
      SopStatuses = sopStatuses
    };
  }

  private async Task<NetworkDetailsModel> GetNetworkDetailsModelAsync(int networkId)
  {
    //prep the SOP Statuses as KeyValuePair for the model
    var sopStatuses = await GetSopStatusKeyValuePairsAsync();

    //having a networkid claim means we can definitely get a network and return a model for it
    var network = await _networkService.Get(networkId);

    //get SOP status desc for current SOP status
    var statusDesc = sopStatuses.FirstOrDefault(x => x.Key == network.SopStatusId).Value;

    return new NetworkDetailsModel
    {
      NetworkId = network.NetworkId,
      NetworkName = network.Name,
      Description = network.Description,
      Url = network.Url,
      ContactEmail = network.Email,
      LogoName = network.Logo,
      SopStatus = network.SopStatusId,
      SopStatusDescription = statusDesc,
      SopStatuses = sopStatuses,
      ContactHandoverEnabled = network.ContactHandoverEnabled,
      HandoverBaseUrl = network.HandoverBaseUrl,
      HandoverOrgIdsUrlParamName = network.HandoverOrgIdsUrlParamName,
      MultipleHandoverOrdIdsParams = network.MultipleHandoverOrdIdsParams,
      HandoverNonMembers = network.HandoverNonMembers,
      HandoverNonMembersUrlParamName = network.HandoverNonMembersUrlParamName
    };
  }

  #region Temp Logo Management
  
  [HttpPost]
  public async Task<ActionResult> AddTempLogo()
  {
    if (!HttpContext.Request.Form.Files.Any())
      return BadRequest(new KeyValuePair<bool, string>(false, "No files found. Please select a new file and try again."));

    var formFile = HttpContext.Request.Form.Files["TempLogo"];

    if (formFile == null)
      return BadRequest(new KeyValuePair<bool, string>(false, "No files found. Please select a new file and try again."));

    if (formFile.Length > 1000000)
      return BadRequest(new KeyValuePair<bool, string>(false, "The file you supplied is too large. Logo image files must be 1Mb or less."));
    
    try
    {
      if (formFile.ValidateAsLogo())
      {
        var logoStream = formFile.ToProcessableStream();
        var resizedImage = await ImageService.ResizeImageStream(logoStream, maxX: 300, maxY: 300);
        // Set session variable so the TempLogo action can retrieve it
        HttpContext.Session.Set("TempLogo", resizedImage.ToArray());

        return
            Ok(new KeyValuePair<bool, string>(true,
                Url.Action("TempLogo", "Profile")));
      }
    }
    catch (BadImageFormatException e)
    {
      return BadRequest(new KeyValuePair<bool, string>(false, e.Message));
    }

    return BadRequest(new KeyValuePair<bool, string>(false, "No files found. Please select a new file and try again."));
  }
  
  [HttpGet]
  public ActionResult TempLogo()
  {
    var bytes = HttpContext.Session.Get("TempLogo");
    return File(bytes, "image/png");
  }

  [HttpPost]
  [Authorize(nameof(AuthPolicies.IsNetworkAdmin))]
  public void RemoveTempLogo()
  {
    HttpContext.Session.Remove("TempLogo");
  }

  #endregion

  #region Biobank membership
  
  [Authorize(nameof(AuthPolicies.HasNetworkClaim))]
  public async Task<ActionResult> Biobanks(int networkId)
  {
    var networkBiobanks =
        await _organisationService.ListByNetworkId(networkId);

    var biobanks = networkBiobanks.Select(x => new NetworkBiobankModel
    {
      BiobankId = x.OrganisationId,
      Name = x.Name
    }).ToList();

    foreach (var biobank in biobanks)
    {
      //get the admins
      biobank.Admins =
          (await _biobankService.ListBiobankAdmins(biobank.BiobankId)).Select(x => x.Email).ToList();

      var organisationNetwork = await _networkService.GetOrganisationNetwork(biobank.BiobankId, networkId);
      biobank.ApprovedDate = organisationNetwork.ApprovedDate;
    }

    //Get OrganisationNetwork with biobankId and networkId

    var model = new NetworkBiobanksModel
    {
      Biobanks = biobanks
    };

    return View(model);
  }
  
  [Authorize(nameof(AuthPolicies.HasNetworkClaim))]
  public async Task<ActionResult> DeleteBiobank(int networkId, int biobankId, string biobankName)
  {
    try
    {
      await _networkService.RemoveOrganisationFromNetwork(biobankId, networkId);

      //send back to the Biobanks list, with feedback (the list may be very long!
      this.SetTemporaryFeedbackMessage(biobankName + " has been removed from your network!", FeedbackMessageType.Success);
    }
    catch
    {
      this.SetTemporaryFeedbackMessage($"{biobankName} could not be deleted.", FeedbackMessageType.Danger);
    }

    return RedirectToAction("Biobanks", new { networkId });
  }

  [Authorize(nameof(AuthPolicies.HasNetworkClaim))]
  public ActionResult AddBiobank(int networkId)
  {
    return View(new AddBiobankToNetworkModel());
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(nameof(AuthPolicies.HasNetworkClaim))]
  public async Task<ActionResult> AddBiobank(int networkId, AddBiobankToNetworkModel model)
  {
    //Ensure biobankName exists (i.e. they've used the typeahead result, not just typed whatever they like)
    var biobank = await _organisationService.GetByName(model.BiobankName);
    
    if (biobank == null)
    {
      this.SetTemporaryFeedbackMessage("We couldn't find a Biobank with the name you entered.", FeedbackMessageType.Danger);
      return View(model);
    }

    var network = await _networkService.Get(networkId);

    //Get all emails from admins and store in list
    var networkAdmins = await GetAdminsAsync(networkId, false);
    var networkEmails = new List<string>();
    foreach (var admin in networkAdmins)
    {
      if (admin.EmailConfirmed == true)
      {
        networkEmails.Add(admin.UserEmail);
      }

    }
    //Add network contact email
    networkEmails.Add(network.Email);
    var biobankAdmins =
        (await _biobankService.ListBiobankAdmins(biobank.OrganisationId))
            .Select(bbAdmin => new RegisterEntityAdminModel
            {
              UserId = bbAdmin.Id,
              UserFullName = bbAdmin.Name,
              UserEmail = bbAdmin.Email,
              EmailConfirmed = bbAdmin.EmailConfirmed
            }).ToList();
    var biobankEmails = new List<string>();

    foreach (var admin in biobankAdmins)
    {
      if (admin.EmailConfirmed == true)
      {
        biobankEmails.Add(admin.UserEmail);
      }
    }
    biobankEmails.Add(biobank.ContactEmail);


    var trustedBiobanks = await _configService.GetSiteConfig(ConfigKey.TrustBiobanks);
    
    if (biobank.IsSuspended)
    {
      this.SetTemporaryFeedbackMessage($"{biobank.Name} cannot be added to a network at this time.", FeedbackMessageType.Danger);
      return View(model);
    }

    try
    {
      var result = false;
      var approved = false;
      if (trustedBiobanks.Value == "true" && networkEmails.Any(biobankEmails.Contains))
      {
        result =
        await
            _networkService.AddOrganisationToNetwork(biobank.OrganisationId,
                networkId, model.BiobankExternalID, true);
        approved = true;
      }
      else
      {
        result =
        await
            _networkService.AddOrganisationToNetwork(biobank.OrganisationId,
        networkId, model.BiobankExternalID, false);
      }

      if (result)
      {
        if (trustedBiobanks.Value == "true" && !approved)
        {
          //Send notification email to biobank
          await _emailService.SendNewBiobankRegistrationNotification(
              new EmailAddress(biobank.ContactEmail),
              model.BiobankName,
              network.Name,
              Url.Action("NetworkAcceptance", "Settings", new { Area = "Biobank", biobankId = biobank.OrganisationId  }, Request.Scheme)
                  );
        }

        //send back to the Biobanks list, with feedback
        this.SetTemporaryFeedbackMessage(model.BiobankName + " has been successfully added to your network!",
            FeedbackMessageType.Success);
        return RedirectToAction("Biobanks", new { networkId });
      }

      this.SetTemporaryFeedbackMessage(model.BiobankName + " is already in your network.",
          FeedbackMessageType.Danger);
    }
    catch
    {
      this.SetTemporaryFeedbackMessage($"{biobank.Name} cannot be added to a network at this time.", FeedbackMessageType.Danger);
    }

    return View(model);
  }
  
  [Authorize(nameof(AuthPolicies.HasNetworkClaim))]
  public async Task<ActionResult> SearchBiobanks(int networkId, string wildcard)
  {
    var biobanks = await _organisationService.List(wildcard, false);

    var biobankResults = biobanks
        .Select(x => new
        {
          Id = x.OrganisationId,
          Name = x.Name
        }).ToList();

    return Ok(biobankResults);
  }

  #endregion

}
