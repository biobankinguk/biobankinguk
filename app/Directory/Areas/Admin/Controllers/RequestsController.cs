using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Data.Entities;
using Biobanks.Directory.Areas.Admin.Models.Requests;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Constants;
using Biobanks.Directory.Models.Emails;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.Directory.Dto;
using Biobanks.Directory.Services.EmailServices.Contracts;
using Biobanks.Directory.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Directory.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
public class RequestsController : Controller
{
  private readonly INetworkService _networkService;
  private readonly IOrganisationDirectoryService _organisationService;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly ITokenLoggingService _tokenLog;
  private readonly IEmailService _emailService;
  private readonly IMapper _mapper;
  public RequestsController(
    INetworkService networkService,
    IOrganisationDirectoryService organisationService,
    UserManager<ApplicationUser> userManager,
    ITokenLoggingService tokenLog,
    IEmailService emailService,
    IMapper mapper
    )
  {
    _networkService = networkService;
    _organisationService = organisationService;
    _userManager = userManager;
    _tokenLog = tokenLog;
    _emailService = emailService;
    _mapper = mapper;
  }
  
  #region Requests
  public async Task<ActionResult> Index()
  {
    var bbRequests =
        (await _organisationService.ListOpenRegistrationRequests())
        .Select(x => new BiobankRequestModel
        {
          RequestId = x.OrganisationRegisterRequestId,
          BiobankName = x.OrganisationName,
          UserName = x.UserName,
          UserEmail = x.UserEmail
        }).ToList();

    var nwRequests =
        (await _networkService.ListOpenRegistrationRequests())
        .Select(x => new NetworkRequestModel
        {
          RequestId = x.NetworkRegisterRequestId,
          NetworkName = x.NetworkName,
          UserName = x.UserName,
          UserEmail = x.UserEmail
        }).ToList();

    var model = new RequestsModel
    {
      BiobankRequests = bbRequests,
      NetworkRequests = nwRequests
    };

    return View(model);
  }

  public async Task<ActionResult> AcceptBiobankRequest(int requestId)
  {
    //Let's fetch the request
    var request = await _organisationService.GetRegistrationRequest(requestId);

    if (request == null)
    {
      this.SetTemporaryFeedbackMessage(
          "That request doesn't exist",
          FeedbackMessageType.Danger);
      return RedirectToAction("Index");
    }

    //what if the request is already accepted/declined?
    if (request.AcceptedDate != null || request.DeclinedDate != null)
    {
      this.SetTemporaryFeedbackMessage(
          $"{request.UserName} ({request.UserEmail}) request for {request.OrganisationName} has already been approved or declined.",
          FeedbackMessageType.Danger);
      return RedirectToAction("Index");
    }

    //try and get the user for the request
    var user = await _userManager.FindByEmailAsync(request.UserEmail);

    // Send email confirming registrations
    if (user is null)
    {
      // Create new user, with new user registration email
      user = new ApplicationUser
      {
        Email = request.UserEmail,
        UserName = request.UserEmail,
        Name = request.UserName
      };

      var result = await _userManager.CreateAsync(user);

      if (!result.Succeeded)
      { 
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("", $"{error.Code}: {error.Description}");
        }

        return View("GlobalErrors");
      }

      // Send email confirmation of registration
      var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

      await _tokenLog.EmailTokenIssued(confirmToken, user.Id);

      await _emailService.SendNewUserRegisterEntityAccepted(
          new EmailAddress(request.UserEmail),
          request.UserName,
          request.OrganisationName,
          Url.Action("Confirm", "Account",
              new
              {
                Area = "",
                userId = user.Id,
                token = confirmToken
              },
              Request.Scheme)
          );
    }
    else
    {
      // Exisiting user - Send email confirmation of registration
      await _emailService.SendExistingUserRegisterEntityAccepted(
          new EmailAddress(request.UserEmail),
          request.UserName,
          request.OrganisationName,
          Url.Action("Create", "Profile",
              new
              {
                Area = "Biobank",
                biobankId = request.OrganisationRegisterRequestId,
                newBiobank = true
              },
              Request.Scheme)
      );
    }
    
    //add user to BiobankAdmin role and sign them out.
    await _userManager.AddToRolesAsync(user,  new List<string> { Role.BiobankAdmin });
    await _userManager.UpdateSecurityStampAsync(user);

    //finally, update the request
    request.AcceptedDate = DateTime.Now;
    await _organisationService.UpdateRegistrationRequest(request);

    //send back, with feedback
    this.SetTemporaryFeedbackMessage(
        $"{request.UserName} ({request.UserEmail}) request for {request.OrganisationName} accepted!",
        FeedbackMessageType.Success);

    return RedirectToAction("Index");
  }

  public async Task<ActionResult> BiobankActivity()
  {
    var activity = new List<BiobankActivityDTO>();

    foreach (var organisation in await _organisationService.ListForActivity(includeSuspended: false))
    {
      var lastActiveUser = await _organisationService.GetLastActiveUser(organisation.OrganisationId);

      activity.Add(new BiobankActivityDTO
      {
        OrganisationId = organisation.OrganisationId,
        Name = organisation.Name,
        ContactEmail = organisation.ContactEmail,
        LastUpdated = organisation.LastUpdated,
        LastCapabilityUpdated = organisation.DiagnosisCapabilities.OrderByDescending(c => c.LastUpdated).FirstOrDefault()?.LastUpdated,
        LastCollectionUpdated = organisation.Collections.OrderByDescending(c => c.LastUpdated).FirstOrDefault()?.LastUpdated,
        LastAdminLoginEmail = lastActiveUser?.Email,
        LastAdminLoginTime = lastActiveUser?.LastLogin
      });
    }

    var model = _mapper.Map<List<BiobankActivityModel>>(activity);

    return View(model);
  }

  public async Task<ActionResult> DeclineBiobankRequest(int requestId)
  {
    //Let's fetch the request
    var request = await _organisationService.GetRegistrationRequest(requestId);

    if (request == null)
    {
      this.SetTemporaryFeedbackMessage(
          "That request doesn't exist",
          FeedbackMessageType.Danger);
      return RedirectToAction("Index");
    }

    //what if the request is already accepted/declined?
    if (request.AcceptedDate != null || request.DeclinedDate != null)
    {
      this.SetTemporaryFeedbackMessage(
          $"{request.UserName} ({request.UserEmail}) request for {request.OrganisationName} has already been approved or declined.",
          FeedbackMessageType.Danger);
      return RedirectToAction("Index");
    }

    //update the request
    request.DeclinedDate = DateTime.Now;
    await _organisationService.UpdateRegistrationRequest(request);

    //send the requester an email
    await _emailService.SendRegisterEntityDeclined(
        new EmailAddress(request.UserEmail),
        request.UserName,
        request.OrganisationName);

    //send back, with feedback
    this.SetTemporaryFeedbackMessage(
        $"{request.UserName} ({request.UserEmail}) request for {request.OrganisationName} declined!",
        FeedbackMessageType.Success);

    return RedirectToAction("Index");
  }

  public async Task<ActionResult> AcceptNetworkRequest(int requestId)
  {
    //Let's fetch the request
    var request = await _networkService.GetRegistrationRequest(requestId);
    if (request == null)
    {
      this.SetTemporaryFeedbackMessage(
          "That request doesn't exist",
          FeedbackMessageType.Danger);
      return RedirectToAction("Index");
    }

    //what if the request is already accepted/declined?
    if (request.AcceptedDate != null || request.DeclinedDate != null)
    {
      this.SetTemporaryFeedbackMessage(
          $"{request.UserName} ({request.UserEmail}) request for {request.NetworkName} has already been approved or declined.",
          FeedbackMessageType.Danger);
      return RedirectToAction("Index");
    }

    //try and get the user for the request
    var user = await _userManager.FindByEmailAsync(request.UserEmail);

    //If necessary, create a new user (with no password, so requiring confirmation/reset)
    if (user == null)
    {
      user = new ApplicationUser
      {
        Email = request.UserEmail,
        UserName = request.UserEmail,
        Name = request.UserName
      };

      var result = await _userManager.CreateAsync(user);

      if (!result.Succeeded)
      {
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError("", $"{error.Code}: {error.Description}");
        }
        return View("GlobalErrors");
      }

      //Send email confirmation of registration
      var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

      await _tokenLog.EmailTokenIssued(confirmToken, user.Id);

      await _emailService.SendNewUserRegisterEntityAccepted(
          new EmailAddress(request.UserEmail),
          request.UserName,
          request.NetworkName,
          Url.Action("Confirm", "Account",
              new
              {
                Area = "",
                userId = user.Id,
                token = confirmToken
              },
              Request.Scheme));
    }
    else
    {
      //Send email confirmation of registration
      await _emailService.SendExistingUserRegisterEntityAccepted(
          new EmailAddress(request.UserEmail),
          request.UserName,
          request.NetworkName,
          Url.Action("Create", "Profile",
                  new
                  {
                    Area = "Network",
                    networkId = request.NetworkRegisterRequestId,
                    newNetwork = true
                  },
                  Request.Scheme)
          );
    }

    //add user to NetworkAdmin role and sign them out.
    await _userManager.AddToRolesAsync(user, new List<string> { Role.NetworkAdmin });
    await _userManager.UpdateSecurityStampAsync(user);

    //finally, update the request
    request.AcceptedDate = DateTime.Now;
    await _networkService.UpdateRegistrationRequest(request);

    //send back, with feedback
    this.SetTemporaryFeedbackMessage(
        $"{request.UserName} ({request.UserEmail}) request for {request.NetworkName} accepted!",
        FeedbackMessageType.Success);

    return RedirectToAction("Index");
  }

  public async Task<ActionResult> DeclineNetworkRequest(int requestId)
  {
    //Let's fetch the request
    var request = await _networkService.GetRegistrationRequest(requestId);
    if (request == null)
    {
      this.SetTemporaryFeedbackMessage(
          "That request doesn't exist",
          FeedbackMessageType.Danger);
      return RedirectToAction("Index");
    }

    //what if the request is already accepted/declined?
    if (request.AcceptedDate != null || request.DeclinedDate != null)
    {
      this.SetTemporaryFeedbackMessage(
          $"{request.UserName} ({request.UserEmail}) request for {request.NetworkName} has already been approved or declined.",
          FeedbackMessageType.Danger);
      return RedirectToAction("Index");
    }

    //update the request
    request.DeclinedDate = DateTime.Now;
    await _networkService.UpdateRegistrationRequest(request);

    //send the requester an email
    await _emailService.SendRegisterEntityDeclined(
        new EmailAddress(request.UserEmail),
        request.UserName,
        request.NetworkName);

    //send back, with feedback
    this.SetTemporaryFeedbackMessage(
        $"{request.UserName} ({request.UserEmail}) request for {request.NetworkName} declined!",
        FeedbackMessageType.Success);

    return RedirectToAction("Index");
  }

  public async Task<ActionResult> ManualActivation(string userEmail)
  {
    var user = await _userManager.FindByEmailAsync(userEmail);

    if (user != null && !user.EmailConfirmed)
    {
      // Generate Token Link
      var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
      var tokenLink = Url.Action("Confirm", "Account",
          new
          {
            Area = "",
            userId = user.Id,
            token = confirmToken
          },
          Request.Scheme);

      // Log Token Issuing
      await _tokenLog.TokenIssued(confirmToken, user.Id, "Manual Account Confirmation");

      // Return Link To User
      return Ok(new { link = tokenLink });
    }

    return NotFound();
  }
  
  #endregion
  
  #region Historical
  
  public async Task<ActionResult> Historical()
  {
      //get both network and biobank historical requests
      //and convert them to the viewmodel format
      var bbRequests = (await _organisationService.ListHistoricalRegistrationRequests())
          .Select(x =>

              Task.Run(async () =>
              {
                  string action;
                  DateTime date;
                  GetHistoricalRequestActionDate(x.DeclinedDate, x.AcceptedDate, out action, out date);
                  var user = await _userManager.FindByEmailAsync(x.UserEmail);

                  return new HistoricalRequestModel
                  {
                      UserName = x.UserName,
                      UserEmail = x.UserEmail,
                      EntityName = x.OrganisationName,
                      Action = action,
                      Date = date,
                      UserEmailConfirmed = user?.EmailConfirmed ?? false,
                      ResultingOrgExternalId = x.OrganisationExternalId
                  };
              }).Result

          ).ToList();

      var nwRequests = (await _networkService.ListHistoricalRegistrationRequests())
          .Select(x =>

              Task.Run(async () =>
              {
                  string action;
                  DateTime date;
                  GetHistoricalRequestActionDate(x.DeclinedDate, x.AcceptedDate, out action, out date);
                  var user = await _userManager.FindByEmailAsync(x.UserEmail);

                  return new HistoricalRequestModel
                  {
                      UserName = x.UserName,
                      UserEmail = x.UserEmail,
                      EntityName = x.NetworkName,
                      Action = action,
                      Date = date,
                      UserEmailConfirmed = user?.EmailConfirmed ?? false
                  };
              }).Result

          ).ToList();

      var model = new HistoricalModel
      {
          HistoricalRequests = bbRequests.Concat(nwRequests).OrderByDescending(x => x.Date).ToList()
      };

      return View(model);
  }

  private static void GetHistoricalRequestActionDate(DateTime? declineDate, DateTime? acceptedDate, out string action, out DateTime date)
  {
      //check it is actually historical
      if (declineDate == null && acceptedDate == null) throw new ApplicationException();

      date = (declineDate ?? acceptedDate).Value;
      action = (declineDate != null) ? "Declined" : "Accepted";
  }

  #endregion
}
