using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Areas.Biobank.Models.Settings;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Constants;
using Biobanks.Submissions.Api.Filters;
using Biobanks.Submissions.Api.Models.Emails;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.EmailServices.Contracts;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Submissions.Api.Areas.Biobank.Controllers;

[Area("Biobank")]
[Authorize(nameof(AuthPolicies.IsBiobankAdmin))]
[SuspendedWarning]
public class SettingsController : Controller
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IBiobankService _biobankService;
  private readonly IEmailService _emailService;
  private readonly IOrganisationDirectoryService _organisationDirectoryService;
  private readonly INetworkService _networkService;
  private readonly IReferenceDataCrudService<AccessCondition> _accessConditionService;
  private readonly IReferenceDataCrudService<CollectionType> _collectionTypeService;
  private readonly ITokenLoggingService _tokenLoggingService;

  public SettingsController(
      UserManager<ApplicationUser> userManager,
      IBiobankService biobankService,
      IEmailService emailService,
      IOrganisationDirectoryService organisationDirectoryService,
      INetworkService networkService,
      IReferenceDataCrudService<AccessCondition> accessConditionService,
      IReferenceDataCrudService<CollectionType> collectionTypeService,
      ITokenLoggingService tokenLoggingService)
  {
      _userManager = userManager;
      _biobankService = biobankService;
      _emailService = emailService;
      _organisationDirectoryService = organisationDirectoryService;
      _networkService = networkService;
      _accessConditionService = accessConditionService;
      _collectionTypeService = collectionTypeService;
      _tokenLoggingService = tokenLoggingService;
  }

        #region Admins  
        [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
        public async Task<ActionResult> Admins(int biobankId)
        {
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

        public ActionResult InviteAdminSuccess(int biobankId, string name)
        {
            //This action solely exists so we can set a feedback message

            this.SetTemporaryFeedbackMessage($"{name} has been successfully added to your admins!",
                FeedbackMessageType.Success);

            return RedirectToAction("Admins", new { biobankId });
        }

        [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
        public async Task<ActionResult> InviteAdminAjax(int biobankId)
        {
            var bb = await _organisationDirectoryService.Get(biobankId);

            return PartialView("_ModalInviteAdmin", new InviteRegisterEntityAdminModel
            {
                Entity = bb.Name,
                EntityName = "biobank",
                ControllerName = "Settings",
                SuccessAreaName = "Biobank",
                SuccessControllerName = "Settings",
                OrganisationId = biobankId
            });
        }

        [HttpPost]
        [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteAdminAjax(InviteRegisterEntityAdminModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
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
                            Request.Protocol));
                }
                else
                {
                    return BadRequest(new
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
                    Url.Action("Index", "Profile", null, Request.Protocol));
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

        [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
        public async Task<ActionResult> DeleteAdmin(int biobankId, string biobankUserId, string userFullName)
        {
            //remove them from the biobank
            await _organisationDirectoryService.RemoveUserFromOrganisation(biobankUserId, biobankId);

            var biobankUser = await _userManager.FindByIdAsync(biobankUserId);

            // Check if they are admin for any other biobanks
            var userBiobanks = await _organisationDirectoryService.ListByUserId(biobankUserId);
            
            if (!userBiobanks.Any())
              // and remove them from the admin role if they are not.
              await _userManager.RemoveFromRolesAsync(biobankUser, new List<string> { Role.BiobankAdmin });

            this.SetTemporaryFeedbackMessage($"{userFullName} has been removed from your admins!", FeedbackMessageType.Success);

            return RedirectToAction("Admins", new { biobankId });
        }
        
        #endregion
        
        #region Submissions
        
        [HttpGet]
        [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
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
        [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submissions(int biobankId, SubmissionsModel model)
        {
            //update Organisations table
            var biobank = await _organisationDirectoryService.Get(biobankId);
            biobank.AccessConditionId = model.AccessCondition;
            biobank.CollectionTypeId = model.CollectionType;

            await _organisationDirectoryService.Update(biobank);

            //Set feedback and redirect
            this.SetTemporaryFeedbackMessage("Submissions settings updated!", FeedbackMessageType.Success);

            return RedirectToAction("Submissions", new { biobankId });
        }

        [HttpPost]
        [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
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
        
        #endregion
        
        #region Network Acceptance

        [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
        public async Task<ActionResult> NetworkAcceptance(int biobankId)
        {
          var organisationNetworks = await _networkService.ListOrganisationNetworks(biobankId);
          var networkList = new List<NetworkAcceptanceModel>();
          foreach (var orgNetwork in organisationNetworks)
          {
            var network = await _networkService.Get(orgNetwork.NetworkId);
            var organisation = new NetworkAcceptanceModel
            {
              BiobankId = biobankId,
              NetworkId = network.NetworkId,
              NetworkName = network.Name,
              NetworkDescription = network.Description,
              NetworkEmail = network.Email,
              ApprovedDate = orgNetwork.ApprovedDate
            };
            networkList.Add(organisation);

          }

          var model = new AcceptanceModel
          {
            NetworkRequests = networkList
          };

          return View(model);
        }

        [Authorize(nameof(AuthPolicies.HasBiobankClaim))]
        public async Task<ActionResult> AcceptNetworkRequest(int biobankId, int networkId)
        {
          var organisationNetwork = await _networkService.GetOrganisationNetwork(biobankId, networkId);

          organisationNetwork.ApprovedDate = DateTime.Now;
          await _networkService.UpdateOrganisationNetwork(organisationNetwork);

          this.SetTemporaryFeedbackMessage("Biobank added to the network successfully", FeedbackMessageType.Success);

          return RedirectToAction("NetworkAcceptance", new { biobankId });
        }
        #endregion
  
}
