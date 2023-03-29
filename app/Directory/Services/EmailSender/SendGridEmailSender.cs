#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Directory.Config;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.EmailServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Biobanks.Directory.Services.EmailSender
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridOptions _config;
        private readonly RazorViewService _emailViews;
        private readonly ILogger<SendGridEmailSender> _logger;
        private readonly SendGridClient _sendGrid;
        private readonly string? _environmentLabel;

        public SendGridEmailSender(
            IOptions<SendGridOptions> options,
            RazorViewService emailViews,
            ILogger<SendGridEmailSender> logger,
            IHostEnvironment env)
        {
            _config = options.Value;
            _emailViews = emailViews;
            _logger = logger;
            _sendGrid = new SendGridClient(_config.SendGridApiKey);

            if (!env.IsProduction())
                _environmentLabel = $"[{env.EnvironmentName}] ";
        }

        /// <summary>
        /// Allows us to add non null labels to the subject line when appropriate
        /// </summary>
        /// <param name="subject">The actual subject line</param>
        /// <returns>The labelled subject line</returns>
        private string LabelledSubject(string? subject)
          => string
              .Join(" ", new List<string?>
                  {
                _environmentLabel,
                subject
                }
                .Where(x => x is not null));

        public async Task SendEmail<TModel>(
            Models.Emails.EmailAddress toAddress,
            string viewName,
            TModel model,
            Models.Emails.EmailAddress? ccAddress = null
            )
            where TModel : class
        {
            List<Models.Emails.EmailAddress> ccAddresses = new();
            if (ccAddress != null)
            {
                ccAddresses.Add(ccAddress);
            }

            await SendEmail(
                new List<Models.Emails.EmailAddress>() { toAddress },
                viewName, model, ccAddresses);
        }

        public async Task SendEmail<TModel>(
            List<Models.Emails.EmailAddress> toAddresses,
            string viewName,
            TModel model,
            List<Models.Emails.EmailAddress>? ccAddresses = null)
            where TModel : class
        {
            var (body, viewContext) = await _emailViews.RenderToString(viewName, model);

            var message = new SendGridMessage
            {
                From = new(_config.FromAddress, _config.FromName),
                ReplyTo = new(_config.ReplyToAddress),
                Subject = LabelledSubject((string?)viewContext.ViewBag.Subject ?? string.Empty),
                HtmlContent = body
            };

            foreach (var address in toAddresses)
                message.AddTo(address.Address, address.Name);

            if (ccAddresses != null)
            {
                foreach (var address in ccAddresses)
                    message.AddCc(address.Address, address.Name);
            }

            var response = await _sendGrid.SendEmailAsync(message);
            var success = ((int)response.StatusCode).ToString().StartsWith("2");
            if (!success)
            {
                var error = $"Error response from SendGrid: {response.StatusCode}";
                _logger.LogError(error);
                _logger.LogError(await response.Body.ReadAsStringAsync());

                // Helpful bits
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    _logger.LogError(
                        $"Have you setup a verified Sender, and does it match the configured FromAddress ({_config.FromAddress})?");

                throw new InvalidOperationException(error);
            }
        }
    }
}
