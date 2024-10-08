using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Constants;
using Biobanks.Directory.Models.Emails;
using Biobanks.Directory.Models.Shared;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.EmailServices.Contracts;
using Biobanks.Directory.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetworkAdminsModel = Biobanks.Directory.Areas.Network.Models.Settings.NetworkAdminsModel;

namespace Biobanks.Directory.Areas.Network.Controllers;

[Area("Network")]
[Authorize(nameof(AuthPolicies.IsNetworkAdmin))]
public class SettingsController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly INetworkService _networkService;
    private readonly ITokenLoggingService _tokenLoggingService;

    public SettingsController(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        INetworkService networkService,
        ITokenLoggingService tokenLoggingService
        )
    {
        _userManager = userManager;
        _emailService = emailService;
        _networkService = networkService;
        _tokenLoggingService = tokenLoggingService;
    }

    [Authorize(nameof(AuthPolicies.HasNetworkClaim))]
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

    public ActionResult InviteAdminSuccess(int networkId, string name)
    {
        //This action solely exists so we can set a feedback message

        this.SetTemporaryFeedbackMessage($"{name} has been successfully added to your network admins!",
            FeedbackMessageType.Success);

        return RedirectToAction("Admins", new { networkId });
    }

    [Authorize(nameof(AuthPolicies.HasNetworkClaim))]
    public async Task<ActionResult> InviteAdminAjax(int networkId)
    {
        var network = await _networkService.Get(networkId);

        return PartialView("_ModalInviteAdmin", new InviteRegisterEntityAdminModel
        {
            Entity = network.Name,
            EntityName = "network",
            ControllerName = "Network"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(nameof(AuthPolicies.HasNetworkClaim))]
    public async Task<ActionResult> InviteAdminAjax(int networkId, InviteRegisterEntityAdminModel model)
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
                        Request.Scheme));
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
                Url.Action("Biobanks", "Profile", new { networkId }, Request.Scheme));
        }

        //Add the user/network relationship
        await _networkService.AddNetworkUser(user.Id, networkId);

        //add user to NetworkAdmin role and sign them out.
        await _userManager.AddToRolesAsync(user, new List<string> {Role.NetworkAdmin});
        await _userManager.UpdateSecurityStampAsync(user);

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

    [Authorize(nameof(AuthPolicies.HasNetworkClaim))]
    public async Task<ActionResult> DeleteAdmin(int networkId, string networkUserId, string userFullName)
    {
        //remove them from the network
        await _networkService.RemoveNetworkUser(networkUserId, networkId);
        
        var networkUser = await _userManager.FindByIdAsync(networkUserId);
        
        // Check if they are admin for any other networks
        var userNetworks = await _networkService.ListByUserId(networkUserId);
        
        if (!userNetworks.Any())
          // and remove them from the admin role if they are not and sign them out.
          await _userManager.RemoveFromRolesAsync(networkUser, new List<string> {Role.NetworkAdmin});
        await _userManager.UpdateSecurityStampAsync(networkUser);

        this.SetTemporaryFeedbackMessage($"{userFullName} has been removed from your network admins!", FeedbackMessageType.Success);

        return RedirectToAction("Admins", new { networkId });
    }
}
