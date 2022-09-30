using Biobanks.Identity.Contracts;
using Biobanks.Identity.Data.Entities;
using Biobanks.Identity.Constants;
using Microsoft.AspNet.Identity;
using Postal;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Biobanks.Services
{
    public class SendGridEmailService : Contracts.IEmailService
    {
        private readonly string _apiKey;
        private readonly string _fromAddress;
        private readonly string _fromName;
        private readonly string _directoryName;
        private readonly Postal.IEmailService _emailService;
        private readonly IApplicationRoleManager<ApplicationRole, IdentityRole> _roleManager;
        private readonly IApplicationUserManager<ApplicationUser, string, IdentityResult> _userManager;

        public SendGridEmailService(
            string apiKey,
            string fromAddress,
            string fromName,
            string directoryName,
            Postal.IEmailService emailService,
            IApplicationRoleManager<ApplicationRole, IdentityRole> roleManager,
            IApplicationUserManager<ApplicationUser, string, IdentityResult> userManager)
        {
            _apiKey = apiKey;
            _fromAddress = fromAddress;
            _fromName = fromName;
            _directoryName = directoryName;
            _emailService = emailService;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ResendAccountConfirmation(string to, string name, string confirmLink)
        {
            var client = new SendGridClient(_apiKey);

            dynamic email = new Email("ResendConfirm");
            email.To = to;
            email.Name = name;
            email.ConfirmLink = confirmLink;

            // Construct the email from razor view
            var message = _emailService.CreateMailMessage(email);

            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(_fromAddress, _fromName),
                Subject = $"{_directoryName} Account Confirmation",
                PlainTextContent = message.Body
            };
            sendGridMessage.AddTo(to);
            await SendEmailAsync(client, sendGridMessage);
        }

        public async Task SendExternalNetworkNonMemberInformation(string to, string biobankName, string biobankAnonymousIdentifier,
            string networkName, string networkContactEmail, string networkDescription)
        {
            var client = new SendGridClient(_apiKey);

            dynamic email = new Email("ExternalNetworkNonMemberInformation");
            email.To = to;
            email.BiobankName = biobankName;
            email.BiobankAnonymousIdentifier = biobankAnonymousIdentifier;
            email.NetworkName = networkName;
            email.NetworkContactEmail = networkContactEmail;
            email.NetworkDescription = networkDescription;

            // Construct the email from razor view
            var message = _emailService.CreateMailMessage(email);

            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(_fromAddress, _fromName),
                Subject = $"{_directoryName} external Network contact",
                PlainTextContent = message.Body
            };
            sendGridMessage.AddTo(to);
            await SendEmailAsync(client, sendGridMessage);
        }

        public async Task SendExistingUserRegisterEntityAccepted(string to, string name, string entity, string link)
        {
            var client = new SendGridClient(_apiKey);

            dynamic email = new Email("ExistingUserRegisterEntityAccepted");
            email.To = to;
            email.Name = name;
            email.Entity = entity;
            email.Link = link;

            // Construct the email from razor view
            var message = _emailService.CreateMailMessage(email);

            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(_fromAddress, _fromName),
                Subject = $"{_directoryName} Registration",
                PlainTextContent = message.Body
            };
            sendGridMessage.AddTo(to);
            await SendEmailAsync(client, sendGridMessage);
        }

        public async Task SendExistingUserRegisterEntityAdminInvite(string to, string name, string entity, string link)
        {
            var client = new SendGridClient(_apiKey);

            dynamic email = new Email("ExistingUserRegisterEntityAdminInvite");
            email.To = to;
            email.Name = name;
            email.Entity = entity;
            email.Link = link;

            // Construct the email from razor view
            var message = _emailService.CreateMailMessage(email);

            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(_fromAddress, _fromName),
                Subject = $"{_directoryName} {entity} Administration",
                PlainTextContent = message.Body
            };
            sendGridMessage.AddTo(to);
            await SendEmailAsync(client, sendGridMessage);
        }

        public async Task SendNewUserRegisterEntityAccepted(string to, string name, string entity, string confirmLink)
        {
            var client = new SendGridClient(_apiKey);

            dynamic email = new Email("NewUserRegisterEntityAccepted");
            email.To = to;
            email.Name = name;
            email.Entity = entity;
            email.ConfirmLink = confirmLink;

            // Construct the email from razor view
            var message = _emailService.CreateMailMessage(email);

            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(_fromAddress, _fromName),
                Subject = $"{_directoryName} Registration",
                PlainTextContent = message.Body
            };
            sendGridMessage.AddTo(to);
            await SendEmailAsync(client, sendGridMessage);
        }

        public async Task SendNewUserRegisterEntityAdminInvite(string to, string name, string entity, string confirmLink)
        {
            var client = new SendGridClient(_apiKey);

            dynamic email = new Email("NewUserRegisterEntityAdminInvite");
            email.To = to;
            email.Name = name;
            email.Entity = entity;
            email.ConfirmLink = confirmLink;

            // Construct the email from razor view
            var message = _emailService.CreateMailMessage(email);

            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(_fromAddress, _fromName),
                Subject = $"{_directoryName} {entity} Administration",
                PlainTextContent = message.Body
            };
            sendGridMessage.AddTo(to);
            await SendEmailAsync(client, sendGridMessage);
        }

        public async Task SendNewBiobankRegistrationNotification(string to, string biobankName, string networkName, string link)
        {
            var client = new SendGridClient(_apiKey);

            dynamic email = new Email("NewBiobankNetworkRegistrationNotification");
            email.To = to;
            email.BiobankName = biobankName;
            email.NetworkName = networkName;
            email.Link = link;

            //Construct the email from razor view
            var message = _emailService.CreateMailMessage(email);

            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(_fromAddress, _fromName),
                Subject = $"{_directoryName} Registration Request",
                PlainTextContent = message.Body
            };
            sendGridMessage.AddTo(to);
            await SendEmailAsync(client, sendGridMessage);
        }

        public async Task SendPasswordReset(string to, string username, string resetLink)
        {
            var client = new SendGridClient(_apiKey);

            dynamic email = new Email("PasswordReset");
            email.To = to;
            email.Username = username;
            email.ResetLink = resetLink;

            // Construct the email from razor view
            var message = _emailService.CreateMailMessage(email);

            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(_fromAddress, _fromName),
                Subject = $"{_directoryName} Password Reset",
                PlainTextContent = message.Body
            };
            sendGridMessage.AddTo(to);
            await SendEmailAsync(client, sendGridMessage);
        }

        public async Task SendRegisterEntityDeclined(string to, string name, string entity)
        {
            var client = new SendGridClient(_apiKey);

            dynamic email = new Email("RegisterEntityDeclined");
            email.To = to;
            email.Name = name;
            email.Entity = entity;

            // Construct the email from razor view
            var message = _emailService.CreateMailMessage(email);

            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(_fromAddress, _fromName),
                Subject = $"{_directoryName} Registration",
                PlainTextContent = message.Body
            };
            sendGridMessage.AddTo(to);
            await SendEmailAsync(client, sendGridMessage);
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

            var client = new SendGridClient(_apiKey);

            dynamic email = new Email("DirectoryAdminNewRegisterRequestNotification");
            email.To = string.Join(",", userEmails);

            email.Requester = requesterName;
            email.RequesterEmail = requesterEmail;
            email.EntityType = entityType;
            email.EntityName = entityName;

            // Construct the email from razor view
            var message = _emailService.CreateMailMessage(email);

            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(_fromAddress, _fromName),
                Subject = $"New {_directoryName} Registration Request",
                PlainTextContent = message.Body
            };

            userEmails.ForEach(x => sendGridMessage.AddTo(x));

            await SendEmailAsync(client, sendGridMessage);
        }

        public async Task SendContactList(string to, string contactlist, bool contactMe)
        {
            var client = new SendGridClient(_apiKey);

            dynamic email = new Email("EmailContactList");
            email.To = to;
            email.ContactList = contactlist;

            // Construct the email from razor view
            var message = _emailService.CreateMailMessage(email);

            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(_fromAddress, _fromName),
                Subject = $"{_directoryName} Contact List",
                PlainTextContent = message.Body
            };
            sendGridMessage.AddTo(to);

            if (contactMe)
                sendGridMessage.AddCc(ConfigurationManager.AppSettings["EmailContactAddress"]);

            await SendEmailAsync(client, sendGridMessage);
        }

        private async Task SendEmailAsync(SendGridClient client, SendGridMessage email)
        {
            try
            {
                await client.SendEmailAsync(email);
            }
            catch (Exception e)
            {
                Trace.TraceError($"The {GetType().Name} threw an exception : { e.StackTrace }"); // Trace is caught by Application Insights
            }
        }

    }
}
