using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Areas.Admin.Controllers;
public class RequestsController : Controller
{
  public async Task<ActionResult> Requests()
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
      return RedirectToAction("Requests");
    }

    //what if the request is already accepted/declined?
    if (request.AcceptedDate != null || request.DeclinedDate != null)
    {
      this.SetTemporaryFeedbackMessage(
          $"{request.UserName} ({request.UserEmail}) request for {request.OrganisationName} has already been approved or declined.",
          FeedbackMessageType.Danger);
      return RedirectToAction("Requests");
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
          ModelState.AddModelError("", error);
        }

        return View("GlobalErrors");
      }

      // Send email confirmation of registration
      var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);

      await _tokenLog.EmailTokenIssued(confirmToken, user.Id);

      await _emailService.SendNewUserRegisterEntityAccepted(
          request.UserEmail,
          request.UserName,
          request.OrganisationName,
          Url.Action("Confirm", "Account",
              new
              {
                userId = user.Id,
                token = confirmToken
              },
              Request.Url.Scheme)
          );
    }
    else
    {
      // Exisiting user - Send email confirmation of registration
      await _emailService.SendExistingUserRegisterEntityAccepted(
          request.UserEmail,
          request.UserName,
          request.OrganisationName,
          Url.Action("SwitchToBiobank", "Account",
              new
              {
                id = request.OrganisationRegisterRequestId,
                newBiobank = true
              },
              Request.Url.Scheme)
      );
    }


    //add user to BiobankAdmin role
    await _userManager.AddToRolesAsync(user.Id, Role.BiobankAdmin.ToString());

    //finally, update the request
    request.AcceptedDate = DateTime.Now;
    await _organisationService.UpdateRegistrationRequest(request);

    //send back, with feedback
    this.SetTemporaryFeedbackMessage(
        $"{request.UserName} ({request.UserEmail}) request for {request.OrganisationName} accepted!",
        FeedbackMessageType.Success);

    return RedirectToAction("Requests");
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
      return RedirectToAction("Requests");
    }

    //what if the request is already accepted/declined?
    if (request.AcceptedDate != null || request.DeclinedDate != null)
    {
      this.SetTemporaryFeedbackMessage(
          $"{request.UserName} ({request.UserEmail}) request for {request.OrganisationName} has already been approved or declined.",
          FeedbackMessageType.Danger);
      return RedirectToAction("Requests");
    }

    //update the request
    request.DeclinedDate = DateTime.Now;
    await _organisationService.UpdateRegistrationRequest(request);

    //send the requester an email
    await _emailService.SendRegisterEntityDeclined(
        request.UserEmail,
        request.UserName,
        request.OrganisationName);

    //send back, with feedback
    this.SetTemporaryFeedbackMessage(
        $"{request.UserName} ({request.UserEmail}) request for {request.OrganisationName} declined!",
        FeedbackMessageType.Success);

    return RedirectToAction("Requests");
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
      return RedirectToAction("Requests");
    }

    //what if the request is already accepted/declined?
    if (request.AcceptedDate != null || request.DeclinedDate != null)
    {
      this.SetTemporaryFeedbackMessage(
          $"{request.UserName} ({request.UserEmail}) request for {request.NetworkName} has already been approved or declined.",
          FeedbackMessageType.Danger);
      return RedirectToAction("Requests");
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
          ModelState.AddModelError("", error);
        }
        return View("GlobalErrors");
      }

      //Send email confirmation of registration
      var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);

      await _tokenLog.EmailTokenIssued(confirmToken, user.Id);

      await _emailService.SendNewUserRegisterEntityAccepted(
          request.UserEmail,
          request.UserName,
          request.NetworkName,
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
      //Send email confirmation of registration
      await _emailService.SendExistingUserRegisterEntityAccepted(
          request.UserEmail,
          request.UserName,
          request.NetworkName,
          Url.Action("SwitchToNetwork", "Account",
                  new
                  {
                    id = request.NetworkRegisterRequestId,
                    newNetwork = true
                  },
                  Request.Url.Scheme)
          );
    }

    //add user to NetworkAdmin role
    await _userManager.AddToRolesAsync(user.Id, Role.NetworkAdmin.ToString());

    //finally, update the request
    request.AcceptedDate = DateTime.Now;
    await _networkService.UpdateRegistrationRequest(request);

    //send back, with feedback
    this.SetTemporaryFeedbackMessage(
        $"{request.UserName} ({request.UserEmail}) request for {request.NetworkName} accepted!",
        FeedbackMessageType.Success);

    return RedirectToAction("Requests");
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
      return RedirectToAction("Requests");
    }

    //what if the request is already accepted/declined?
    if (request.AcceptedDate != null || request.DeclinedDate != null)
    {
      this.SetTemporaryFeedbackMessage(
          $"{request.UserName} ({request.UserEmail}) request for {request.NetworkName} has already been approved or declined.",
          FeedbackMessageType.Danger);
      return RedirectToAction("Requests");
    }

    //update the request
    request.DeclinedDate = DateTime.Now;
    await _networkService.UpdateRegistrationRequest(request);

    //send the requester an email
    await _emailService.SendRegisterEntityDeclined(
        request.UserEmail,
        request.UserName,
        request.NetworkName);

    //send back, with feedback
    this.SetTemporaryFeedbackMessage(
        $"{request.UserName} ({request.UserEmail}) request for {request.NetworkName} declined!",
        FeedbackMessageType.Success);

    return RedirectToAction("Requests");
  }

  public async Task<ActionResult> ManualActivation(string userEmail)
  {
    var user = await _userManager.FindByEmailAsync(userEmail);

    if (user != null && !user.EmailConfirmed)
    {
      // Generate Token Link
      var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
      var tokenLink = Url.Action("Confirm", "Account",
          new
          {
            userId = user.Id,
            token = confirmToken
          },
          Request.Url.Scheme);

      // Log Token Issuing
      await _tokenLog.TokenIssued(confirmToken, user.Id, "Manual Account Confirmation");

      // Return Link To User
      return Json(new { link = tokenLink }, JsonRequestBehavior.AllowGet);
    }

    return new HttpNotFoundResult();
  }
}
