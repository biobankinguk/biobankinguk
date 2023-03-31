#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Biobanks.Directory.Config;
using Biobanks.Directory.Models.Emails;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.EmailServices;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Biobanks.Directory.Services.EmailSender
{
    public class LocalDiskEmailSender : IEmailSender
    {
        private readonly LocalDiskEmailOptions _config;
        private readonly string _localPath;
        private readonly RazorViewService _emailViews;

        public LocalDiskEmailSender(
            IOptions<LocalDiskEmailOptions> options,
            RazorViewService emailViews)
        {
            _config = options.Value;
            _emailViews = emailViews;

            // local path preprocessing
            // special case replacements, like `~`
            _localPath = _config.LocalPath.StartsWith("~/")
                    ? Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                _config.LocalPath.Replace("~/", ""))
                : _config.LocalPath;
        }

        private void EnsureTargetPath()
        {
            if (!System.IO.Directory.Exists(_localPath))
                System.IO.Directory.CreateDirectory(_localPath);
        }

        /// <inheritdoc />
        public async Task SendEmail<TModel>(List<EmailAddress> toAddresses, string viewName, TModel model, List<EmailAddress>? ccAddresses = null)
            where TModel : class
        {
            EnsureTargetPath();

            var (body, viewContext) = await _emailViews.RenderToString(viewName, model);

            var message = new MimeMessage();

            foreach (var address in toAddresses)
                message.To.Add(!string.IsNullOrEmpty(address.Name)
                    ? new MailboxAddress(address.Name, address.Address)
                    : MailboxAddress.Parse(address.Address));

            message.From.Add(new MailboxAddress(_config.FromName, _config.FromAddress));
            message.ReplyTo.Add(MailboxAddress.Parse(_config.ReplyToAddress));
            message.Subject = (string?)viewContext.ViewBag.Subject ?? string.Empty;

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };

            if (ccAddresses != null)
            {
                foreach (var address in ccAddresses)
                    message.Cc.Add(!string.IsNullOrEmpty(address.Name)
                        ? new MailboxAddress(address.Name, address.Address)
                        : MailboxAddress.Parse(address.Address));
            }


            await message.WriteToAsync(
                Path.Combine(_localPath,
                    MessageFileName(viewName, toAddresses[0].Address)));
        }

        public async Task SendEmail<TModel>(EmailAddress toAddress, string viewName, TModel model, EmailAddress? ccAddress = null)
            where TModel : class
        {
            List<EmailAddress> ccAddresses = new();
            if (ccAddress != null)
            {
                ccAddresses.Add(ccAddress);
            }
            await SendEmail(new List<EmailAddress> { toAddress }, viewName, model, ccAddresses);
        }

        private static string ShortViewName(string viewName)
            => viewName[(viewName.LastIndexOf('/') + 1)..];

        private static string SafeIsoDate(DateTimeOffset date)
            => date.ToString("o").Replace(":", "-");

        private static string MessageFileName(string viewName, string recipient)
            => $"{ShortViewName(viewName)}_{recipient}_{SafeIsoDate(DateTimeOffset.UtcNow)}.eml";

    }
}
