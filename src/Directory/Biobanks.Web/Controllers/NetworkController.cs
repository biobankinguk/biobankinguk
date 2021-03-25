using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Biobanks.Entities.Data;
using Biobanks.Identity.Data.Entities;
using Biobanks.Identity.Contracts;
using Biobanks.Identity.Constants;
using Biobanks.Services;
using Biobanks.Web.Models.Network;
using Biobanks.Directory.Data.Transforms.Url;
using Biobanks.Services.Contracts;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Utilities;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using AutoMapper;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Services.Dto;
using Biobanks.Web.Extensions;
using Biobanks.Web.Filters;
using Biobanks.Directory.Data.Constants;
using Newtonsoft.Json;

namespace Biobanks.Web.Controllers
{
    [UserAuthorize(Roles = "NetworkAdmin")]
    public class NetworkController : ApplicationBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        private readonly IApplicationUserManager<ApplicationUser, string, IdentityResult> _userManager;
        private readonly IEmailService _emailService;
        private readonly CustomClaimsManager _claimsManager;

        private readonly IMapper _mapper;
        private readonly ITokenLoggingService _tokenLog;

        private const string TempNetworkLogoSessionId = "TempNetworkLogo";
        private const string TempNetworkLogoContentTypeSessionId = "TempNetworkLogoContentType";

        public NetworkController(
            IBiobankReadService biobankReadService,
            IBiobankWriteService biobankWriteService,
            IApplicationUserManager<ApplicationUser, string, IdentityResult> userManager,
            IEmailService emailService,
            CustomClaimsManager claimsManager,
            ITokenLoggingService tokenLog,
            IMapper mapper)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
            _userManager = userManager;
            _emailService = emailService;
            _claimsManager = claimsManager;
            _tokenLog = tokenLog;
            _mapper = mapper;
        }

        #region Details

        [Authorize(ClaimType = CustomClaimType.Network)]
        public async Task<ActionResult> Index()
        {
            return View(await GetNetworkDetailsModelAsync());
        }

        public async Task<ActionResult> Edit(bool detailsIncomplete = false)
        {
            if (detailsIncomplete)
                SetTemporaryFeedbackMessage("Please fill in the details below for your network. Once you have completed these, you'll be able to perform other administration tasks",
                    FeedbackMessageType.Info);

            var activeOrganisationType = Convert.ToInt32(Session[SessionKeys.ActiveOrganisationType]);

            return activeOrganisationType == (int)ActiveOrganisationType.NewNetwork
                ? View(await NewNetworkDetailsModelAsync()) //no network id means we're dealing with a request
                : View(await GetNetworkDetailsModelAsync()); //network id means we're dealing with an existing network
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
                await _biobankWriteService.RemoveLogoAsync(model.NetworkId.Value);
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

            //Build entities from the model
            var network = new Network
            {
                //id is set only if we are editing, not creating
                //same with external id
                //and type
                Name = model.NetworkName,
                Description = model.Description,
                Url = model.Url,
                Email = model.ContactEmail,
                Logo = logoName,
                SopStatusId = model.SopStatus
            };

            if (create)
            {
                network = await _biobankWriteService.CreateNetworkAsync(network);
                await _biobankWriteService.AddUserToNetworkAsync(User.Identity.GetUserId(), network.NetworkId);

                //update the request to show network created
                var request = await _biobankReadService.GetNetworkRegisterRequestByUserEmailAsync(User.Identity.Name);
                request.NetworkCreatedDate = DateTime.Now;
                await _biobankWriteService.UpdateNetworkRegisterRequestAsync(request);

                //add a claim now that they're associated with the network
                _claimsManager.AddClaims(new List<Claim>
                {
                    new Claim(CustomClaimType.Network, JsonConvert.SerializeObject(new KeyValuePair<int, string>(network.NetworkId, network.Name)))
                });

                //Logo upload (now we have the id, we can form the filename)

                Session[SessionKeys.ActiveOrganisationType] = ActiveOrganisationType.Network;
                Session[SessionKeys.ActiveOrganisationId] = network.NetworkId;
                Session[SessionKeys.ActiveOrganisationName] = network.Name;

                if (model.Logo != null)
                {
                    try
                    {
                        network.Logo = await UploadNetworkLogoAsync(model.Logo, network.NetworkId);
                        var networkDto = _mapper.Map<NetworkDTO>(network);
                        await _biobankWriteService.UpdateNetworkAsync(networkDto);
                    }
                    catch (ArgumentNullException)
                    {
                    } //no problem, just means no logo uploaded in this form submission
                }
            }
            else
            {
                //add Update only bits of the Entity
                network.NetworkId = model.NetworkId.Value;

                // update handover Url components
                network.ContactHandoverEnabled = model.ContactHandoverEnabled;
                network.HandoverBaseUrl = model.HandoverBaseUrl;
                network.HandoverOrgIdsUrlParamName = model.HandoverOrgIdsUrlParamName;
                network.MultipleHandoverOrdIdsParams = model.MultipleHandoverOrdIdsParams;
                network.HandoverNonMembers = model.HandoverNonMembers;
                network.HandoverNonMembersUrlParamName = model.HandoverNonMembersUrlParamName;

                var networkDto = _mapper.Map<NetworkDTO>(network);

                await _biobankWriteService.UpdateNetworkAsync(networkDto);
            }

            SetTemporaryFeedbackMessage("Network details updated!", FeedbackMessageType.Success);

            //Back to the profile to view your saved changes
            return RedirectToAction("Index");
        }

        private async Task<string> UploadNetworkLogoAsync(HttpPostedFileBase logo, int? networkId)
        {
            const string networkLogoPrefix = "NETWORK-";

            var logoStream = logo.ToProcessableStream();

            var logoName =
                await
                    _biobankWriteService.StoreLogoAsync(
                        logoStream,
                        logo.FileName,
                        logo.ContentType,
                        networkLogoPrefix + networkId);
            return logoName;
        }

        private async Task<List<KeyValuePair<int, string>>> GetSopStatusKeyValuePairsAsync()
        {
            var allSopStatuses = (List<SopStatus>) await _biobankReadService.ListSopStatusesAsync();
            return
                allSopStatuses.Select(status => new KeyValuePair<int, string>(status.Id, status.Value))
                    .ToList();
        }

        private async Task<NetworkDetailsModel> NewNetworkDetailsModelAsync()
        {
            //prep the SOP Statuses as KeyValuePair for the model
            var sopStatuses = await GetSopStatusKeyValuePairsAsync();

            //Network doesn't exist yet, but the request does, so get the name
            var request = await _biobankReadService.GetNetworkRegisterRequestAsync(SessionHelper.GetNetworkId(Session));

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
            var network = await _biobankReadService.GetNetworkByIdAsync(SessionHelper.GetNetworkId(Session));

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

        #endregion


        #region Temp Logo Management

        [HttpPost]
        public JsonResult AddTempLogo()
        {
            if (!System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                return Json(new KeyValuePair<bool, string>(false, "No files found. Please select a new file and try again."));

            var fileBase = System.Web.HttpContext.Current.Request.Files["TempLogo"];

            if (fileBase == null)
                return Json(new KeyValuePair<bool, string>(false, "No files found. Please select a new file and try again."));

            if (fileBase.ContentLength > 1000000)
                return Json(new KeyValuePair<bool, string>(false, "The file you supplied is too large. Logo image files must be 1Mb or less."));

            var fileBaseWrapper = new HttpPostedFileWrapper(fileBase);

            try
            {
                if (fileBaseWrapper.ValidateAsLogo())
                {
                    var logoStream = fileBaseWrapper.ToProcessableStream();
                    Session[TempNetworkLogoSessionId] =
                        ImageService.ResizeImageStream(logoStream, maxX: 300, maxY: 300)
                        .ToArray();
                    Session[TempNetworkLogoContentTypeSessionId] = fileBaseWrapper.ContentType;

                    return
                        Json(new KeyValuePair<bool, string>(true,
                            Url.Action("TempLogo", "Network")));
                }
            }
            catch (BadImageFormatException e)
            {
                return Json(new KeyValuePair<bool, string>(false, e.Message));
            }

            return Json(new KeyValuePair<bool, string>(false, "No files found. Please select a new file and try again."));
        }

        [HttpGet]
        public ActionResult TempLogo(string id)
        {
            return File((byte[])Session[TempNetworkLogoSessionId], Session[TempNetworkLogoContentTypeSessionId].ToString());
        }

        [HttpPost]
        public void RemoveTempLogo()
        {
            Session[TempNetworkLogoSessionId] = null;
            Session[TempNetworkLogoContentTypeSessionId] = null;
        }

        #endregion


        #region Admins

        [Authorize(ClaimType = CustomClaimType.Network)]
        public async Task<ActionResult> Admins()
        {
            var networkId = SessionHelper.GetNetworkId(Session);

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
                (await _biobankReadService.ListNetworkAdminsAsync(networkId))
                    .Select(nwAdmin => new RegisterEntityAdminModel
                    {
                        UserId = nwAdmin.Id,
                        UserFullName = nwAdmin.Name,
                        UserEmail = nwAdmin.Email,
                        EmailConfirmed = nwAdmin.EmailConfirmed
                    }).ToList();

            if (excludeCurrentUser)
            {
                admins.Remove(admins.FirstOrDefault(x => x.UserId == CurrentUser.Identity.GetUserId()));
            }

            return admins;
        }

        public async Task<JsonResult> GetAdminsAjax(int networkId, bool excludeCurrentUser = false, int timeStamp = 0)
        {
            //timeStamp can be used to avoid caching issues, notably on IE

            return Json(await GetAdminsAsync(networkId, excludeCurrentUser), JsonRequestBehavior.AllowGet);
        }

        public ActionResult InviteAdminSuccess(string name)
        {
            //This action solely exists so we can set a feedback message

            SetTemporaryFeedbackMessage($"{name} has been successfully added to your network admins!",
                FeedbackMessageType.Success);

            return RedirectToAction("Admins");
        }

        public async Task<ActionResult> InviteAdminAjax(int networkId)
        {
            var nw = await _biobankReadService.GetNetworkByIdAsync(networkId);

            return PartialView("_ModalInviteAdmin", new InviteRegisterEntityAdminModel
            {
                Entity = nw.Name,
                EntityName = "network",
                ControllerName = "Network"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(ClaimType = CustomClaimType.Network)]
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

            var networkId = (await _biobankReadService.GetNetworkByNameAsync(model.Entity)).NetworkId;
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
                    Url.Action("Index", "Network", null, Request.Url.Scheme));
            }

            //Add the user/network relationship
            await _biobankWriteService.AddUserToNetworkAsync(user.Id, networkId);

            //add user to NetworkAdmin role
            await _userManager.AddToRolesAsync(user.Id, Role.NetworkAdmin.ToString());

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

        [Authorize(ClaimType = CustomClaimType.Network)]
        public async Task<ActionResult> DeleteAdmin(string networkUserId, string userFullName)
        {
            //remove them from the network
            await _biobankWriteService.RemoveUserFromNetworkAsync(networkUserId, SessionHelper.GetNetworkId(Session));

            //and remove them from the role, since they can only be admin of one network at a time, and we just removed it!
            await _userManager.RemoveFromRolesAsync(networkUserId, Role.NetworkAdmin.ToString());

            SetTemporaryFeedbackMessage($"{userFullName} has been removed from your network admins!", FeedbackMessageType.Success);

            return RedirectToAction("Admins");
        }

        #endregion


        #region Biobank membership

        [Authorize(ClaimType = CustomClaimType.Network)]
        public async Task<ActionResult> Biobanks()
        {
            var networkId = SessionHelper.GetNetworkId(Session);
            var networkBiobanks =
                (await _biobankReadService.GetBiobanksByNetworkIdAsync(SessionHelper.GetNetworkId(Session))).ToList();

            var biobanks = networkBiobanks.Select(x => new NetworkBiobankModel
            {
                BiobankId = x.OrganisationId,
                Name = x.Name
            }).ToList();

            foreach (var biobank in biobanks)
            {
                //get the admins
                biobank.Admins =
                    (await _biobankReadService.ListBiobankAdminsAsync(biobank.BiobankId)).Select(x => x.Email).ToList();

                var organisationNetwork = await _biobankReadService.GetOrganisationNetworkAsync(biobank.BiobankId, networkId);
                biobank.ApprovedDate = organisationNetwork.First().ApprovedDate;
            }

            //Get OrganisationNetwork with biobankId and networkId

            var model = new NetworkBiobanksModel
            {
                Biobanks = biobanks
            };

            return View(model);
        }

        [Authorize(ClaimType = CustomClaimType.Network)]
        public async Task<ActionResult> DeleteBiobank(int biobankId, string biobankName)
        {
            try
            {
                await _biobankWriteService.RemoveBiobankFromNetworkAsync(biobankId, SessionHelper.GetNetworkId(Session));
                
                //send back to the Biobanks list, with feedback (the list may be very long!
                SetTemporaryFeedbackMessage(biobankName + " has been removed from your network!", FeedbackMessageType.Success);
            }
            catch
            {
                SetTemporaryFeedbackMessage($"{biobankName} could not be deleted.", FeedbackMessageType.Danger);
            }
            
            return RedirectToAction("Biobanks");
        }

        [Authorize(ClaimType = CustomClaimType.Network)]
        public ActionResult AddBiobank()
        {
            return View(new AddBiobankToNetworkModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(ClaimType = CustomClaimType.Network)]
        public async Task<ActionResult> AddBiobank(AddBiobankToNetworkModel model)
        {
            //Ensure biobankName exists (i.e. they've used the typeahead result, not just typed whatever they like)
            var biobank = await _biobankReadService.GetBiobankByNameAsync(model.BiobankName);

            var networkId = SessionHelper.GetNetworkId(Session);
            var network = await _biobankReadService.GetNetworkByIdAsync(networkId);

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
                (await _biobankReadService.ListBiobankAdminsAsync(biobank.OrganisationId))
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


            Config trustedBiobanks = await _biobankReadService.GetSiteConfig(ConfigKey.TrustBiobanks);

            if (biobank == null)
            {
                SetTemporaryFeedbackMessage("We couldn't find a Biobank with the name you entered.", FeedbackMessageType.Danger);
                return View(model);
            }

            if (biobank.IsSuspended)
            {
                SetTemporaryFeedbackMessage($"{biobank.Name} cannot be added to a network at this time.", FeedbackMessageType.Danger);
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
                        _biobankWriteService.AddBiobankToNetworkAsync(biobank.OrganisationId,
                            SessionHelper.GetNetworkId(Session), model.BiobankExternalID, true);
                    approved = true;
                }
                else
                {
                    result =
                    await
                        _biobankWriteService.AddBiobankToNetworkAsync(biobank.OrganisationId,
                    SessionHelper.GetNetworkId(Session), model.BiobankExternalID, false);
                }

                if (result)
                {
                    if (trustedBiobanks.Value == "true" && !approved)
                    {
                        //Send notification email to biobank
                        await _emailService.SendNewBiobankRegistrationNotification(
                            biobank.ContactEmail,
                            model.BiobankName,
                            network.Name,
                            Url.Action("NetworkAcceptance", "Biobank", null, Request.Url.Scheme)
                                );
                    }
                   
                    //send back to the Biobanks list, with feedback
                    SetTemporaryFeedbackMessage(model.BiobankName + " has been successfully added to your network!",
                        FeedbackMessageType.Success);
                    return RedirectToAction("Biobanks");
                }

                SetTemporaryFeedbackMessage(model.BiobankName + " is already in your network.",
                    FeedbackMessageType.Danger);
            }
            catch
            {
                SetTemporaryFeedbackMessage($"{biobank.Name} cannot be added to a network at this time.", FeedbackMessageType.Danger);
            }

            return View(model);
        }

        [Authorize(ClaimType = CustomClaimType.Network)]
        public async Task<JsonResult> SearchBiobanks(string wildcard)
        {
            var biobanks = await _biobankReadService.ListBiobanksAsync(wildcard, false);

            var biobankResults = biobanks
                .Select(x => new
                {
                    Id = x.OrganisationId,
                    Name = x.Name
                }).ToList();

            return Json(biobankResults, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
