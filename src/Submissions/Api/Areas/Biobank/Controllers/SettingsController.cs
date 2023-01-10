using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Areas.Biobank.Models.Settings;
using Biobanks.Submissions.Api.Constants;
using Biobanks.Submissions.Api.Models.Emails;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.EmailServices;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Submissions.Api.Areas.Biobank.Controllers;

[Area("Biobank")]
public class SettingsController : Controller
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IBiobankService _biobankService;
  private readonly EmailService _emailService;
  private readonly IOrganisationDirectoryService _organisationDirectoryService;
  private readonly IReferenceDataService<AccessCondition> _accessConditionService;
  private readonly IReferenceDataService<CollectionType> _collectionTypeService;
  private readonly ITokenLoggingService _tokenLoggingService;

  public SettingsController(
      UserManager<ApplicationUser> userManager,
      IBiobankService biobankService,
      EmailService emailService,
      IOrganisationDirectoryService organisationDirectoryService,
      IReferenceDataService<AccessCondition> accessConditionService,
      IReferenceDataService<CollectionType> collectionTypeService,
      ITokenLoggingService tokenLoggingService)
  {
      _userManager = userManager;
      _biobankService = biobankService;
      _emailService = emailService;
      _organisationDirectoryService = organisationDirectoryService;
      _accessConditionService = accessConditionService;
      _collectionTypeService = collectionTypeService;
      _tokenLoggingService = tokenLoggingService;
  }

        [Authorize(CustomClaimType.Biobank)]
        public async Task<ActionResult> Admins(int biobankId)
        {
            if (biobankId == 0)
                return RedirectToAction("Index", "Home");

            return View(new BiobankAdminsModel
            {
                BiobankId = biobankId,
                Admins = await GetAdminsAsync(biobankId, excludeCurrentUser: true),
                RequestUrl = HttpContext.Request.GetEncodedUrl()
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
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                admins.Remove(admins.FirstOrDefault(x => x.UserId == currentUser.Id));
            }

            return admins;
        }

        public async Task<ActionResult> GetAdminsAjax(int biobankId, bool excludeCurrentUser = false, int timeStamp = 0)
        {
            //timeStamp can be used to avoid caching issues, notably on IE
            
            return Ok(await GetAdminsAsync(biobankId, excludeCurrentUser));
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
        public async Task<ActionResult> InviteAdminAjax(InviteRegisterEntityAdminModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new
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
                    var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    await _tokenLoggingService.EmailTokenIssued(confirmToken, user.Id);

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
                    return Ok(new
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
                    Url.Action("Index", "Profile", null, Request.GetEncodedUrl()));
            }

            //Add the user/biobank relationship
            await _organisationDirectoryService.AddUserToOrganisation(user.Id, biobankId);
            
            //add user to BiobankAdmin role
            await _userManager.AddToRolesAsync(user, new List<string> { Role.BiobankAdmin }); //what happens if they're already in the role?

            //return success, and enough user details for adding to the viewmodel's list
            return Ok(new
            {
                success = true,
                userId = user.Id,
                name = user.Name,
                email = user.Email,
                emailConfirmed = user.EmailConfirmed
            });
        }

        [Authorize(CustomClaimType.Biobank)]
        public async Task<ActionResult> DeleteAdmin(int biobankId, string biobankUserId, string userFullName)
        {

            if (biobankId == 0)
                return RedirectToAction("Index", "Home");

            //remove them from the network
            await _organisationDirectoryService.RemoveUserFromOrganisation(biobankUserId, biobankId);

            var biobankUser = await _userManager.FindByIdAsync(biobankUserId);

            //and remove them from the role, since they can only be admin of one network at a time, and we just removed it!
            await _userManager.RemoveFromRolesAsync(biobankUser, new List<string> { Role.BiobankAdmin });

            this.SetTemporaryFeedbackMessage($"{userFullName} has been removed from your admins!", FeedbackMessageType.Success);

            return RedirectToAction("Admins");
        }
        
        [HttpGet]
        [Authorize(CustomClaimType.Biobank)]
        public async Task<ActionResult> Submissions(int biobankId)
        {
            var model = new SubmissionsModel();

            //populate drop downs
            model.AccessConditions = (await _accessConditionService.List())
                .Select(x => new ReferenceDataModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    SortOrder = x.SortOrder
                }).OrderBy(x => x.SortOrder);

            model.CollectionTypes = (await _collectionTypeService.List())
                .Select(x => new ReferenceDataModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    SortOrder = x.SortOrder
                }).OrderBy(x => x.SortOrder);

            //get currently selected values from org (if applicable)
            var biobank = await _organisationDirectoryService.GetForBulkSubmissions(biobankId);

            model.BiobankId = biobankId;
            model.AccessCondition = biobank.AccessConditionId;
            model.CollectionType = biobank.CollectionTypeId;
            model.ClientId = biobank.ApiClients.FirstOrDefault()?.ClientId;

            return View(model);
        }

        [HttpPost]
        [Authorize(CustomClaimType.Biobank)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submissions(SubmissionsModel model, int biobankId)
        {
            //update Organisations table
            var biobank = await _organisationDirectoryService.Get(biobankId);
            biobank.AccessConditionId = model.AccessCondition;
            biobank.CollectionTypeId = model.CollectionType;

            await _organisationDirectoryService.Update(biobank);

            //Set feedback and redirect
            this.SetTemporaryFeedbackMessage("Submissions settings updated!", FeedbackMessageType.Success);

            return RedirectToAction("Submissions");
        }

        [HttpPost]
        [Authorize(CustomClaimType.Biobank)]
        public async Task<ActionResult> GenerateApiKeyAjax(int biobankId)
        {
            var credentials =
                await _organisationDirectoryService.IsApiClient(biobankId)
                    ? await _organisationDirectoryService.GenerateNewSecretForBiobank(biobankId)
                    : await _organisationDirectoryService.GenerateNewApiClient(biobankId);

            return Ok(new
            {
                ClientId = credentials.Key,
                ClientSecret = credentials.Value
            });
        }
  
}