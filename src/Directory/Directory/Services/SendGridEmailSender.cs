using System.Threading.Tasks;
using Directory.Config;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using UoN.AspNetCore.RazorViewRenderer;

namespace Directory.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridOptions _config;
        private readonly IRazorViewRenderer _emailViews;
        private readonly SendGridClient _sendGrid;

        public string FromName { get; set; }
        public string FromAddress { get; set; }

        public SendGridEmailSender(
            IOptions<SendGridOptions> options,
            IRazorViewRenderer emailViews)
        {
            _config = options.Value;
            FromName = _config.FromName;
            FromAddress = _config.FromAddress;
            _emailViews = emailViews;
            _sendGrid = new SendGridClient(_config.SendGridApiKey);
        }

        public async Task SendEmail<TModel>(string toAddress, string subject, string viewName, TModel model, string? toName = null)
            where TModel : class
        {
            var message = new SendGridMessage
            {
                From = new EmailAddress(FromAddress, FromName),
                Subject = subject,
                PlainTextContent = await _emailViews.AsString(
                    viewName,
                    model)
            };
            message.AddTo(toAddress, toName);
            await _sendGrid.SendEmailAsync(message);
        }
    }
}
