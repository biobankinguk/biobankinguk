using Biobanks.Data.Entities;
using Biobanks.Submissions.Api.Constants;
using Biobanks.Submissions.Api.Models.Biobank;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.EmailServices.Contracts;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers.Directory;
public class BiobankController : Controller
{

  private readonly BiobankService _biobankService;
  private readonly OrganisationDirectoryService _organisationDirectoryService;
  private readonly IEmailService _emailService;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly ITokenLoggingService _tokenLog;

  public BiobankController(BiobankService biobankService, OrganisationDirectoryService organisationDirectoryService, IEmailService emailService, UserManager<ApplicationUser> userManager, ITokenLoggingService tokenLog)
  {
    _biobankService = biobankService;
    _organisationDirectoryService = organisationDirectoryService;
    _emailService = emailService;
    _userManager = userManager;
    _tokenLog = tokenLog;
  }

  [Authorize(CustomClaimType.Biobank)]
  public async Task<ActionResult> Admins()
  {
    var biobankId = SessionHelper.GetBiobankId(Session);

    if (biobankId == 0)
      return RedirectToAction("Index", "Home");

    return View(new BiobankAdminsModel
    {
      BiobankId = biobankId,
      Admins = await GetAdminsAsync(biobankId, excludeCurrentUser: true)
    });
  }

  private async Task<List<RegisterEntityAdminModel>> GetAdminsAsync(int biobankId, bool excludeCurrentUser)
  {
    //we exclude the current user when we are making the list for them
    //but we may want the full list in other circumstances

    var admins =
        (await _biobankService.ListBiobankAdminsAsync(biobankId))
            .Select(bbAdmin => new RegisterEntityAdminModel
            {
              UserId = bbAdmin.Id,
              UserFullName = bbAdmin.Name,
              UserEmail = bbAdmin.Email,
              EmailConfirmed = bbAdmin.EmailConfirmed
            }).ToList();

    if (excludeCurrentUser)
    {
      admins.Remove(admins.FirstOrDefault(x => x.UserId == CurrentUser.Identity.GetUserId()));
    }

    return admins;
  }

  public async Task<ActionResult> GetAdminsAjax(int biobankId, bool excludeCurrentUser = false, int timeStamp = 0)
  {
    //timeStamp can be used to avoid caching issues, notably on IE
    

    var Admin =  await GetAdminsAsync(biobankId, excludeCurrentUser);

    return Ok(Admin);
  }

  public ActionResult InviteAdminSuccess(string name)
  {
    //This action solely exists so we can set a feedback message

    this.SetTemporaryFeedbackMessage($"{name} has been successfully added to your admins!",
        FeedbackMessageType.Success);

    return RedirectToAction("Admins");
  }

  public async Task<ActionResult> InviteAdminAjax(int biobankId)
  {
    var bb = await _organisationDirectoryService.Get(biobankId);

    return PartialView("_ModalInviteAdmin", new InviteRegisterEntityAdminModel
    {
      Entity = bb.Name,
      EntityName = "biobank",
      ControllerName = "Biobank"
    });
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
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

    var biobankId = (await _organisationDirectoryService.GetByName(model.Entity)).OrganisationId;
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
        var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);

        await _tokenLog.EmailTokenIssued(confirmToken, user.Id);

        await _emailService.SendNewUserRegisterEntityAdminInvite(
            model.Email,
            model.Name,
            model.Entity,
            Url.Action("Confirm", "Account",
                new
                {
                  userId = user.Id,
                  token = confirmToken
                },
                Request.Url.Scheme));
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
          model.Email,
          model.Name,
          model.Entity,
          Url.Action("Index", "Biobank", null, Request.Url.Scheme));
    }

    //Add the user/biobank relationship
    await _organisationDirectoryService.AddUserToOrganisation(user.Id, biobankId);

    //add user to BiobankAdmin role
    await _userManager.AddToRolesAsync(user.Id, Role.BiobankAdmin.ToString()); //what happens if they're already in the role?

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

  [Authorize(CustomClaimType.Biobank)]
  public async Task<ActionResult> DeleteAdmin(string biobankUserId, string userFullName)
  {
    var biobankId = SessionHelper.GetBiobankId(Session);

    if (biobankId == 0)
      return RedirectToAction("Index", "Home");

    //remove them from the network
    await _organisationDirectoryService.RemoveUserFromOrganisation(biobankUserId, biobankId);

    //and remove them from the role, since they can only be admin of one network at a time, and we just removed it!
    await _userManager.RemoveFromRolesAsync(biobankUserId, Role.BiobankAdmin.ToString());

    this.SetTemporaryFeedbackMessage($"{userFullName} has been removed from your admins!", FeedbackMessageType.Success);

    return RedirectToAction("Admins");
  }

}
