using Directory.Identity.Contracts;
using Directory.Identity.Constants;
using Directory.Identity.Data.Entities;
using Microsoft.AspNet.Identity;
using Postal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Configuration;

namespace Directory.Services
{
    public class EmailService : Contracts.IEmailService
    {
        private readonly Postal.IEmailService _service;
        private readonly IApplicationRoleManager<ApplicationRole> _roleManager;
        private readonly IApplicationUserManager<ApplicationUser, string, IdentityResult> _userManager;

        public EmailService(
            Postal.IEmailService service,
            IApplicationRoleManager<ApplicationRole> roleManager,
            IApplicationUserManager<ApplicationUser, string, IdentityResult> userManager)
        {
            _service = service;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SendDirectoryAdminNewRegisterRequestNotification(string requesterName, string requesterEmail, string entityName, string entityType)
        {
            // List of ADAC users
            var userEmails = _roleManager
                .Roles.Single(x => x.Name == nameof(Role.ADAC))
                .Users.Select(x => x.UserId)
                .Aggregate(new List<string>(), (result, id) =>
                {
                    result.Add(_userManager.FindById(id).Email);
                    return result;
                });

            // Fallback on ADAC Support if no ADAC users
            if (userEmails.Count == 0)
            {
                userEmails.Add(ConfigurationManager.AppSettings["AdacSupportEmail"]);
            }

            dynamic email = new Email("DirectoryAdminNewRegisterRequestNotification");
            email.To = string.Join(",", userEmails);

            email.Requester = requesterName;
            email.RequesterEmail = requesterEmail;
            email.EntityType = entityType;
            email.EntityName = entityName;

            await _service.SendAsync(email);
        }

        public async Task SendPasswordReset(string to, string username, string resetLink)
        {
            dynamic email = new Email("PasswordReset");
            email.To = to;
            email.Username = username;
            email.ResetLink = resetLink;
            await SendEmailAsync(email);
        }

        public async Task SendNewBiobankRegistrationNotification(string to, string biobankName, string networkName, string link)
        {
            dynamic email = new Email("NewBiobankNetworkRegistrationNotification");
            email.To = to;
            email.BiobankName = biobankName;
            email.NetworkName = networkName;
            email.Link = link;
            await SendEmailAsync(email);
        }

        public async Task SendExternalNetworkNonMemberInformation(string to, string biobankName,
            string biobankAnonymousIdentifier, string networkName, string networkContactEmail, string networkDescription)
        {
            dynamic email = new Email("ExternalNetworkNonMemberInformation");
            email.To = to;
            email.BiobankName = biobankName;
            email.BiobankAnonymousIdentifier = biobankAnonymousIdentifier;
            email.NetworkName = networkName;
            email.NetworkContactEmail = networkContactEmail;
            email.NetworkDescription = networkDescription;
            await SendEmailAsync(email);
        }

        public async Task SendNewUserRegisterEntityAdminInvite(string to, string name, string entity, string confirmLink)
        {
            dynamic email = new Email("NewUserRegisterEntityAdminInvite");
            email.To = to;
            email.Name = name;
            email.Entity = entity;
            email.ConfirmLink = confirmLink;
            await SendEmailAsync(email);
        }

        public async Task SendNewUserRegisterEntityAccepted(string to, string name, string entity, string confirmLink)
        {
            dynamic email = new Email("NewUserRegisterEntityAccepted");
            email.To = to;
            email.Name = name;
            email.Entity = entity;
            email.ConfirmLink = confirmLink;
            await SendEmailAsync(email);
        }

        public async Task SendExistingUserRegisterEntityAdminInvite(string to, string name, string entity, string link)
        {
            dynamic email = new Email("ExistingUserRegisterEntityAdminInvite");
            email.To = to;
            email.Name = name;
            email.Entity = entity;
            email.Link = link;
            await SendEmailAsync(email);
        }

        public async Task SendExistingUserRegisterEntityAccepted(string to, string name, string entity, string link)
        {
            dynamic email = new Email("ExistingUserRegisterEntityAccepted");
            email.To = to;
            email.Name = name;
            email.Entity = entity;
            email.Link = link;
            await SendEmailAsync(email);
        }

        public async Task SendRegisterEntityDeclined(string to, string name, string entity)
        {
            dynamic email = new Email("RegisterEntityDeclined");
            email.To = to;
            email.Name = name;
            email.Entity = entity;
            await SendEmailAsync(email);
        }

        public async Task ResendAccountConfirmation(string to, string name, string confirmLink)
        {
            dynamic email = new Email("ResendConfirm");
            email.To = to;
            email.Name = name;
            email.ConfirmLink = confirmLink;
            await SendEmailAsync(email);
        }

        public async Task SendContactList(string to, string contactlist, bool contactMe)
        {
            dynamic email = new Email("EmailContactList");
            email.To = to;
            email.ContactList = contactlist;

            if (contactMe)
                email.Cc = ConfigurationManager.AppSettings["EmailContactAddress"];

            await SendEmailAsync(email);
        }

        private async Task SendEmailAsync(Email email)
        {
            try
            {
                await _service.SendAsync(email);
            }
            catch (Exception e)
            {
                Trace.TraceError($"The {GetType().Name} threw an exception : { e.StackTrace }"); // Trace is caught by Application Insights
            }
        }
    }
}
