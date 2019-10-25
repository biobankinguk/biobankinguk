using System;
using System.IO;
using System.Threading.Tasks;
using Directory.Config;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using UoN.AspNetCore.RazorViewRenderer;

namespace Directory.Services
{
    public class LocalDiskEmailSender : IEmailSender
    {
        private readonly LocalMailOptions _config;
        private readonly IRazorViewRenderer _emailViews;

        public string FromName { get; set; }
        public string FromAddress { get; set; }

        public LocalDiskEmailSender(IOptions<LocalMailOptions> options, IRazorViewRenderer emailViews)
        {
            _config = options.Value;
            FromName = _config.FromName;
            FromAddress = _config.FromAddress;
            _emailViews = emailViews;
        }

        /// <inheritdoc />
        public async Task SendEmail<TModel>(string toAddress, string subject, string viewName, TModel model, string? toName)
        {
            var message = new MimeMessage();
            message.To.Add(!string.IsNullOrEmpty(toName)
                ? new MailboxAddress(toName, toAddress)
                : new MailboxAddress(toAddress));
            message.From.Add(new MailboxAddress(FromName, FromAddress));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = await _emailViews.AsString(viewName, model)
            };

            await message.WriteToAsync(
                Path.Combine(_config.LocalPath,
                $"{nameof(model).Replace("Model", "")}_{toName ?? toAddress}_{DateTimeOffset.UtcNow}.eml"));
        }
    }
}
