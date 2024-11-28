using System;
using Biobanks.Directory.Config;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.EmailSender;
using Biobanks.Directory.Services.EmailServices;
using Biobanks.Directory.Services.EmailServices.Contracts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Biobanks.Directory.Startup.ConfigureServicesExtensions
{
  public static class EmailSenderServices
  {
    /// <summary>
    /// Enum for Email Providers.
    /// </summary>
    enum EmailProviders
    {
      Local,
      SendGrid,
      SMTP
    }

    /// <summary>
    /// Parse the desired Email Provider from configuration.
    /// </summary>
    /// <param name="c">The configuration object</param>
    /// <returns>The <c>enum</c> for the chosen Email Provider</returns>
    private static EmailProviders GetEmailProvider(IConfiguration c)
    {
      // TryParse defaults failures to the first enum value.
      Enum.TryParse<EmailProviders>(
        c["OutboundEmail:Provider"],
        ignoreCase: true,
        out var emailProvider);

      return emailProvider;
    }

    private static IServiceCollection ConfigureEmail(
      this IServiceCollection s,
      EmailProviders provider,
      IConfiguration c)
    {
      // set the appropriate configuration function
      Func<IConfiguration, IServiceCollection> configureEmail =
        provider switch
        {
          EmailProviders.SendGrid => s.Configure<SendGridOptions>,
          EmailProviders.SMTP => s.Configure<SmtpOptions>,
          _ => s.Configure<LocalDiskEmailOptions>
        };

      // and execute it
      configureEmail.Invoke(c.GetSection("OutboundEmail"));

      return s;
    }

    /// <summary>
    /// Determine which email sender the user wishes to use and adds it the service collection.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static IServiceCollection AddEmailSender(this IServiceCollection s, IConfiguration c)
    {
      var emailProvider = GetEmailProvider(c);

      s.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

      return s.ConfigureEmail(emailProvider, c)
        .AddTransient<RazorViewService>()
        .AddTransient<AccountEmailService>()
        .AddTransient<IEmailService, EmailService>()
        .AddTransient(
          typeof(IEmailSender),
          emailProvider switch
          {
            EmailProviders.SendGrid => typeof(SendGridEmailSender),
            // EmailProviders.SMTP => 
            _ => typeof(LocalDiskEmailSender)
          });
    }
  }
}
