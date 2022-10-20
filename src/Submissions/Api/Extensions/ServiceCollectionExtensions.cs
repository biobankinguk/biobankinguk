using System;
using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.EmailSender;
using Biobanks.Submissions.Api.Services.EmailServices;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Biobanks.Submissions.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailSender(this IServiceCollection s, IConfiguration c)
        {

            var emailProvider = c["OutboundEmail:Provider"] ?? string.Empty;

            var useSendGrid = emailProvider.Equals("sendgrid", StringComparison.InvariantCultureIgnoreCase);

            if (useSendGrid) s.Configure<SendGridOptions>(c.GetSection("OutboundEmail"));
            else s.Configure<LocalDiskEmailOptions>(c.GetSection("OutboundEmail"));

            s
                .AddTransient<RazorViewService>()
                .AddTransient<AccountEmailService>()
                .TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            if (useSendGrid) s.AddTransient<IEmailSender, SendGridEmailSender>();
            else s.AddTransient<IEmailSender, LocalDiskEmailSender>();

            return s;
        }
    }
}
