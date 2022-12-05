using Biobanks.Data.Entities;
using Biobanks.Submissions.Api.Constants;
using Biobanks.Submissions.Api.Models.Emails;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.EmailServices.Contracts;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers.Directory;
public class NetworkController : Controller
{
  private readonly INetworkService _networkService;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly ITokenLoggingService _tokenLog;
  private readonly IEmailService _emailService;

  public NetworkController(
    INetworkService networkService,
    UserManager<ApplicationUser> userManager,
     ITokenLoggingService tokenLog,
     IEmailService emailService)
  {
    _networkService = networkService;
    _userManager = userManager;
    _tokenLog = tokenLog;
    _emailService = emailService;
  }

  [Authorize(CustomClaimType.Network)]
  public async Task<ActionResult> Admins(int networkId)
  {

    return View(new NetworkAdminsModel
    {
      NetworkId = networkId,
      Admins = await GetAdminsAsync(networkId, excludeCurrentUser: true)
    });
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
      var currentUser = await _userManager.GetUserAsync(HttpContext.User);
      admins.Remove(admins.FirstOrDefault(x => x.UserId == currentUser.Id));
    }

    return admins;
  }

  public async Task<IActionResult> GetAdminsAjax(int networkId, bool excludeCurrentUser = false, int timeStamp = 0)
  {
    //timeStamp can be used to avoid caching issues, notably on IE

    return Ok(await GetAdminsAsync(networkId, excludeCurrentUser));
  }

  public ActionResult InviteAdminSuccess(string name)
  {
    //This action solely exists so we can set a feedback message

   this.SetTemporaryFeedbackMessage($"{name} has been successfully added to your network admins!",
        FeedbackMessageType.Success);

    return RedirectToAction("Admins");
  }

  public async Task<ActionResult> InviteAdminAjax(int networkId)
  {
    var nw = await _networkService.Get(networkId);

    return PartialView("_ModalInviteAdmin", new InviteRegisterEntityAdminModel
    {
      Entity = nw.Name,
      EntityName = "network",
      ControllerName = "Network"
    });
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [Authorize(CustomClaimType.Network)]
  public async Task<JsonResult> InviteAdminAjax(InviteRegisterEntityAdminModel model)
  {
    if (!ModelState.IsValid)
    {
      return Json(new
      {
        success = false,
        errors = ModelState.Values
              .Where(x => x.Errors.Count > 0)
              .SelectMany(x => x.Errors)
              .Select(x => x.ErrorMessage).ToList()
      });
    }

    var networkId = (await _networkService.GetByName(model.Entity)).NetworkId;
    var user = await _userManager.FindByEmailAsync(model.Email);

    if (user == null)
    {
      //User doesn't exist; add a new one
      //Create a new user, no password at this time (so they don't really function yet)
      user = new ApplicationUser
      {
        Email = model.Email,
        UserName = model.Email,
        Name = model.Name
      };

      var result = await _userManager.CreateAsync(user);

      if (result.Succeeded)
      {
        //send email to confirm account
        var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        await _tokenLog.EmailTokenIssued(confirmToken, user.Id);

        await _emailService.SendNewUserRegisterEntityAdminInvite(
            new EmailAddress(model.Email),
            model.Name,
            model.Entity,
            Url.Action("Confirm", "Account",
                new
                {
                  userId = user.Id,
                  token = confirmToken
                },
                Request.GetEncodedUrl()));
      }
      else
      {
        return Json(new
        {
          success = false,
          errors = result.Errors.ToArray()
        });
      }

      //Maybe there should be an auth (or shared) method for all this
    }
    else
    {
      //send email to inform the existing user they've been added as an admin
      await _emailService.SendExistingUserRegisterEntityAdminInvite(
          new EmailAddress(model.Email),
          model.Name,
          model.Entity,
          Url.Action("Index", "Biobank", null, Request.GetEncodedUrl()));
    }

    //Add the user/network relationship
    await _networkService.AddNetworkUser(user.Id, networkId);

    //add user to NetworkAdmin role
    await _userManager.AddToRolesAsync(user, new List<string> { Role.BiobankAdmin });

    //return success, and enough user details for adding to the viewmodel's list
    return Json(new
    {
      success = true,
      userId = user.Id,
      name = user.Name,
      email = user.Email,
      emailConfirmed = user.EmailConfirmed
    });
  }

  [Authorize(CustomClaimType.Network)]
  public async Task<ActionResult> DeleteAdmin(string networkUserId, string userFullName, int networkId)
  {
    //remove them from the network
    await _networkService.RemoveNetworkUser(networkUserId, networkId);

    //and remove them from the role, since they can only be admin of one network at a time, and we just removed it!
    await _userManager.RemoveFromRolesAsync(await _userManager.FindByIdAsync(networkUserId), new List<string> { Role.NetworkAdmin });

    this.SetTemporaryFeedbackMessage($"{userFullName} has been removed from your network admins!", FeedbackMessageType.Success);

    return RedirectToAction("Admins");
  }
}
