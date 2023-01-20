using Biobanks.Data.Entities;

using Biobanks.Submissions.Api.Constants;
using Biobanks.Submissions.Api.Models.Emails;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.EmailServices.Contracts;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Submissions.Api.Areas.Admin.Models.Biobank;
using Microsoft.AspNetCore.Http.Extensions;

namespace Biobanks.Submissions.Api.Areas.Admin.Controllers;

[Area("Admin")]
public class BiobanksController : Controller
{
  private readonly BiobankService _biobankService;
  private readonly OrganisationDirectoryService _organisationService;
  private readonly IEmailService _emailService;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly ITokenLoggingService _tokenLog;
  private readonly Mapper _mapper;

  public BiobanksController(
    BiobankService biobankService,
    OrganisationDirectoryService organisationDirectoryService, 
    IEmailService emailService, 
    UserManager<ApplicationUser> userManager, 
    ITokenLoggingService tokenLog, 
    Mapper mapper
    )
  {
    _biobankService = biobankService;
    _organisationService = organisationDirectoryService;
    _emailService = emailService;
    _userManager = userManager;
    _tokenLog = tokenLog;
    _mapper = mapper;
  }
  
  public async Task<ActionResult> BiobankAdmin(int id = 0)
  {
      var biobank = await _organisationService.Get(id);

      if (biobank != null)
      {
          var model = _mapper.Map<BiobankModel>(biobank);

          //get the admins
          model.Admins =
              (await _biobankService.ListBiobankAdminsAsync(model.BiobankId)).Select(x => new RegisterEntityAdminModel
              {
                  UserId = x.Id,
                  UserFullName = x.Name,
                  UserEmail = x.Email,
                  EmailConfirmed = x.EmailConfirmed
              }).ToList();

          return View(model);
      }
      else
          return NotFound();
  }

  public async Task<ActionResult> DeleteAdmin(int biobankId, string biobankUserId)
  {
      if (biobankId == 0)
          return RedirectToAction("Index", "Home");

      var userFullName =
          (await _biobankService.ListBiobankAdminsAsync(biobankId)).Select(x => new RegisterEntityAdminModel
          {
              UserId = x.Id,
              UserFullName = x.Name,
              UserEmail = x.Email,
              EmailConfirmed = x.EmailConfirmed
          }).SingleOrDefault(x => x.UserId == biobankUserId)
          ?.UserFullName;

      //remove them from the network
      await _organisationService.RemoveUserFromOrganisation(biobankUserId, biobankId);
      
      //and remove them from the role, since they can only be admin of one network at a time, and we just removed it!
      var user = await _userManager.FindByIdAsync(biobankUserId);
      await _userManager.RemoveFromRolesAsync(user, new[] { Role.BiobankAdmin });

      this.SetTemporaryFeedbackMessage($"{userFullName} has been removed from the admins!", FeedbackMessageType.Success);

      return RedirectToAction("BiobankAdmin", new { id = biobankId });
  }

  public async Task<ActionResult> Biobanks()
  {
      var allBiobanks =
          (await _organisationService.List()).ToList();

      var biobanks = allBiobanks.Select(x => new BiobankModel
      {
          BiobankId = x.OrganisationId,
          BiobankExternalId = x.OrganisationExternalId,
          Name = x.Name,
          IsSuspended = x.IsSuspended,
          ContactEmail = x.ContactEmail
      }).ToList();

      foreach (var biobank in biobanks)
      {
          //get the admins
          biobank.Admins =
              (await _biobankService.ListBiobankAdminsAsync(biobank.BiobankId)).Select(x => new RegisterEntityAdminModel
              {
                  UserId = x.Id,
                  UserFullName = x.Name,
                  UserEmail = x.Email,
                  EmailConfirmed = x.EmailConfirmed
              }).ToList();
      }

      var model = new BiobanksModel
      {
          Biobanks = biobanks
      };

      return View(model);
  }

  public async Task<ActionResult> InviteAdminAjax(int biobankId)
  {
      var bb = await _organisationService.Get(biobankId);

      return PartialView("_ModalInviteAdmin", new InviteRegisterEntityAdminModel
      {
          Entity = bb.Name,
          EntityName = "biobank",
          ControllerName = "ADAC"
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

      var biobankId = (await _organisationService.GetByName(model.Entity)).OrganisationId;
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
              Url.Action("BiobankAdmin", "Biobanks", new { id = biobankId }, Request.GetEncodedUrl()));
      }

      //Add the user/biobank relationship
      await _organisationService.AddUserToOrganisation(user.Id, biobankId);

      //add user to BiobankAdmin role
      await _userManager.AddToRolesAsync(user, new[] { Role.BiobankAdmin }); //what happens if they're already in the role?

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

  [HttpGet]
  public async Task<ActionResult> DeleteBiobank(int id)
  {
      var biobank = await _organisationService.Get(id);

      if (biobank != null) return View(_mapper.Map<BiobankModel>(biobank));

      this.SetTemporaryFeedbackMessage("The selected biobank could not be found.", FeedbackMessageType.Danger);
      return RedirectToAction("Biobanks");
  }

  [HttpPost]
  public async Task<ActionResult> DeleteBiobank(BiobankModel model)
  {
      try
      {
          // remove the biobank itself
          var biobank = await _organisationService.Get(model.BiobankId);
          var usersInBiobank = await _biobankReadService.ListSoleBiobankAdminIdsAsync(model.BiobankId);
          await _organisationService.Delete(model.BiobankId);

          // remove admin role from users who had admin role only for this biobank
          foreach (var user in usersInBiobank)
          {
              await _userManager.RemoveFromRolesAsync(user, new[] { Role.BiobankAdmin });
          }

          //remove biobank registration request to allow re-registration 
          var biobankRequest = await _organisationService.GetRegistrationRequestByName(biobank.Name);
          await _organisationService.RemoveRegistrationRequest(biobankRequest);
          this.SetTemporaryFeedbackMessage($"{biobank.Name} and its associated data has been deleted.", FeedbackMessageType.Success);
      }
      catch
      {

          this.SetTemporaryFeedbackMessage("The selected biobank could not be deleted.", FeedbackMessageType.Danger);
      }

      return RedirectToAction("Biobanks");
  }

  public async Task<ActionResult> SuspendBiobank(int id)
  {
      try
      {
          var biobank = await _organisationService.Suspend(id);
          if (biobank.IsSuspended)
              this.SetTemporaryFeedbackMessage($"{biobank.Name} has been suspended.", FeedbackMessageType.Success);
      }
      catch
      {
          this.SetTemporaryFeedbackMessage("The selected biobank could not be suspended.", FeedbackMessageType.Danger);
      }

      return RedirectToAction($"BiobankAdmin", new { id });
  }

  public async Task<ActionResult> UnsuspendBiobank(int id)
  {
      try
      {
          var biobank = await _organisationService.Unsuspend(id);
          if (!biobank.IsSuspended)
              this.SetTemporaryFeedbackMessage($"{biobank.Name} has been unsuspended.", FeedbackMessageType.Success);
      }
      catch
      {
          this.SetTemporaryFeedbackMessage("The selected biobank could not be unsuspended.", FeedbackMessageType.Danger);
      }

      return RedirectToAction($"BiobankAdmin", new { id });
  }

  public async Task<ActionResult> GenerateResetLinkAjax(string biobankUserId, string biobankUsername)
  {
      // Get the reset token
      var resetToken = await _biobankReadService.GetUnusedTokenByUser(biobankUserId);
      await _tokenLog.PasswordTokenIssued(resetToken, biobankUserId);

      // Generate the reset URL
      var url = Url.Action("ResetPassword", "Account",
          new { userId = biobankUserId, token = resetToken },
          Request.GetEncodedUrl());

      return PartialView("_ModalResetPassword", new ResetPasswordEntityModel
      {
          ResetLink = url,
          UserName = biobankUsername
      });
  }

}
