using Biobanks.Data.Entities;
using Biobanks.Data.Transforms.Url;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Areas.Admin.Models.Network;
using Biobanks.Submissions.Api.Constants;
using Biobanks.Submissions.Api.Extensions;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.Directory.Dto;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Areas.Network.Controllers;
public class ProfileController : Controller
{
  private INetworkService _networkService;
  private ILogoStorageProvider _logoStorageProvider;
  private readonly IReferenceDataService<SopStatus> _sopStatusService;
  private readonly UserClaimsPrincipalFactory<ApplicationUser, IdentityRole> _claimsManager;
  private readonly UserManager<ApplicationUser> _userManager;

  public ProfileController(INetworkService networkService, ILogoStorageProvider logoStorageProvider, 
    IReferenceDataService<SopStatus> sopStatusService, UserClaimsPrincipalFactory<ApplicationUser, IdentityRole> claimsManager,
    UserManager<ApplicationUser> userManager)
  {
    _networkService = networkService;
    _logoStorageProvider = logoStorageProvider;
    _sopStatusService = sopStatusService;
    _claimsManager = claimsManager;
    _userManager = userManager;
  }

  [AllowAnonymous]
  //[Authorize(CustomClaimType.Network)]
  public async Task<ActionResult> Index()
  {
    return View(await GetNetworkDetailsModelAsync());
  }

  public async Task<ActionResult> Edit(bool detailsIncomplete = false)
  {
    if (detailsIncomplete)
      this.SetTemporaryFeedbackMessage("Please fill in the details below for your network. Once you have completed these, you'll be able to perform other administration tasks",
          FeedbackMessageType.Info);

    //ToDo: Session
    /*   var activeOrganisationType = Convert.ToInt32(Session[SessionKeys.ActiveOrganisationType]);

        return activeOrganisationType == (int)ActiveOrganisationType.NewNetwork
            ? View(await NewNetworkDetailsModelAsync()) //no network id means we're dealing with a request
            : View(await GetNetworkDetailsModelAsync()); //network id means we're dealing with an existing network*/

    return View(await NewNetworkDetailsModelAsync());
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
      await _logoStorageProvider.RemoveLogoAsync(model.NetworkId.Value);
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
      await _networkService.AddNetworkUser(currentUser.Id, networkDto.NetworkId);

      //update the request to show network created
      var request = await _networkService.GetRegistrationRequestByEmail(User.Identity.Name);
      request.NetworkCreatedDate = DateTime.Now;
      await _networkService.UpdateRegistrationRequest(request);

      //add a claim now that they're associated with the network
      _claimsManager.AddClaims(new List<Claim>
                {
                    new Claim(CustomClaimType.Network, JsonConvert.SerializeObject(new KeyValuePair<int, string>(networkDto.NetworkId, networkDto.Name)))
                });

      //Logo upload (now we have the id, we can form the filename)

      Session[SessionKeys.ActiveOrganisationType] = ActiveOrganisationType.Network;
      Session[SessionKeys.ActiveOrganisationId] = networkDto.NetworkId;
      Session[SessionKeys.ActiveOrganisationName] = networkDto.Name;

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
    return RedirectToAction("Index");
  }

  private async Task<string> UploadNetworkLogoAsync(IFormFile logo, int? networkId)
  {
    const string networkLogoPrefix = "NETWORK-";

    var logoStream = logo.ToProcessableStream();

    var logoName =
        await
            _logoStorageProvider.StoreLogoAsync(
                logoStream,
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

  private async Task<NetworkDetailsModel> NewNetworkDetailsModelAsync()
  {
    //prep the SOP Statuses as KeyValuePair for the model
    var sopStatuses = await GetSopStatusKeyValuePairsAsync();

    //Network doesn't exist yet, but the request does, so get the name
    var request = await _networkService.GetRegistrationRequest(SessionHelper.GetNetworkId(Session));

    //validate that the request is accepted
    if (request.AcceptedDate == null) return null;

    return new NetworkDetailsModel
    {
      NetworkName = request.NetworkName,
      SopStatuses = sopStatuses
    };
  }

  private async Task<NetworkDetailsModel> GetNetworkDetailsModelAsync()
  {
    //prep the SOP Statuses as KeyValuePair for the model
    var sopStatuses = await GetSopStatusKeyValuePairsAsync();

    //having a networkid claim means we can definitely get a network and return a model for it
    var network = await _networkService.Get(SessionHelper.GetNetworkId(Session));

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
}
