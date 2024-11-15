using Biobanks.Directory.Services.EmailServices;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System;
using Biobanks.Directory.Config;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Models.Emails;
using MailKit.Security;

namespace Biobanks.Directory.Services.EmailSender;

public class SmtpEmailSender : IEmailSender
{
  private readonly SmtpOptions _config;
  private readonly RazorViewService _emailViews;
  private readonly ILogger<SmtpEmailSender> _logger;
  private readonly MailKit.Net.Smtp.SmtpClient _smtpClient;

  public SmtpEmailSender(
    IOptions<SmtpOptions> options,
    RazorViewService emailViews,
    ILogger<SmtpEmailSender> logger)
  {
    _config = options.Value;
    _emailViews = emailViews;
    _logger = logger;
    _smtpClient = new MailKit.Net.Smtp.SmtpClient();
  }

  public async Task SendEmail<TModel>(List<EmailAddress> toAddresses, string viewName, TModel model, List<EmailAddress>? ccAddresses = null)
    where TModel : class
  {
    toAddresses = toAddresses.Where(x => !_config.ExcludedEmailAddresses.Contains(x.Address)).ToList(); // filter out blocked email addresses

    if (toAddresses.Count == 0) return;

    var (body, viewContext) = await _emailViews.RenderToString(viewName, model);

    var message = new MimeMessage();

    foreach (var address in toAddresses)
      message.To.Add(!string.IsNullOrEmpty(address.Name)
        ? new MailboxAddress(address.Name, address.Address)
        : MailboxAddress.Parse(address.Address));

    message.From.Add(new MailboxAddress(_config.FromName, _config.FromAddress));
    message.ReplyTo.Add(MailboxAddress.Parse(_config.ReplyToAddress));
    message.Subject = (string)viewContext.ViewBag.Subject ?? string.Empty;

    message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
    {
      Text = body
    };

    var smtpOptions = IsSmtpOptionsAvailable(); // check if relevant SMTP options are available
    if (!smtpOptions) LogError("SMTP values missing or incomplete"); // if not, log custom error message

    ConnectAndAuthenticate(); // connect and authenticate 
    SendEmailMessage(message); // send email
  }

  public async Task SendEmail<TModel>(EmailAddress toAddress, string viewName, TModel model, EmailAddress? ccAddress = null)
    where TModel : class
    => await SendEmail(new List<EmailAddress> { toAddress }, viewName, model);

  private bool IsSmtpOptionsAvailable()
  {
    // checking if the correct numeric values (port no. and secure socket enum) are supplied
    if (_config.SmtpPort == 0 || _config.SmtpSecureSocketEnum is > 4 or < 1) return false;

    // if okay, then check string values (hostname, username & password) 
    string[] smtpStringOptions = { _config.SmtpHost, _config.SmtpUsername, _config.SmtpPassword };
    return !smtpStringOptions.Any(string.IsNullOrEmpty);
  }

  private void ConnectAndAuthenticate()
  {
    // Anything above 4 or less than 1, use 1(Auto)
    var secureSocketKey = _config.SmtpSecureSocketEnum is > 4 or < 1 ? 1 : _config.SmtpSecureSocketEnum;

    var secureSocketOption = new Dictionary<int, SecureSocketOptions>
    {
      { 1, SecureSocketOptions.Auto },
      { 2, SecureSocketOptions.SslOnConnect },
      { 3, SecureSocketOptions.StartTls },
      { 4, SecureSocketOptions.StartTlsWhenAvailable }
    };

    try
    {
      _smtpClient.Connect(_config.SmtpHost, _config.SmtpPort, secureSocketOption[secureSocketKey]);
      _smtpClient.Authenticate(_config.SmtpUsername, _config.SmtpPassword);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex.Message); // log exception message
      LogError("Couldn't connect or authenticate"); // log custom error message
    }

  }
  private void SendEmailMessage(MimeMessage message)
  {
    try { _smtpClient.Send(message); }
    catch (Exception ex)
    {
      _logger.LogError(ex.Message); // log exception message
      LogError("Couldn't send a message");// log custom error message
    }
    finally
    {
      _smtpClient.Disconnect(true);
      _smtpClient.Dispose();
    }
  }

  private void LogError(string customErrorMsg)
  {
    _logger.LogError(customErrorMsg);
    throw new InvalidOperationException(customErrorMsg); // then throw exception
  }



}

