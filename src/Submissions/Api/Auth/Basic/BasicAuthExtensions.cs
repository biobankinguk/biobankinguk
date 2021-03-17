using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System;

namespace Biobanks.Submissions.Api.Auth.Basic
{
    // based on
    // https://joonasw.net/view/creating-auth-scheme-in-aspnet-core-2
    // though that abstracts the credential checking into a service
    // and genericises these extensions to specify the service

    public static class BasicAuthExtensions
    {
        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder)
            => AddBasic(builder, BasicAuthDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, string authenticationScheme)
            => AddBasic(builder, authenticationScheme, _ => { });
        
        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, Action<BasicAuthSchemeOptions> configureOptions)
            => AddBasic(builder, BasicAuthDefaults.AuthenticationScheme, configureOptions);
        
        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, string authenticationScheme, Action<BasicAuthSchemeOptions> configureOptions)
        {
            builder.Services.AddSingleton<IPostConfigureOptions<BasicAuthSchemeOptions>, BasicAuthPostConfigureOptions>();

            return builder.AddScheme<BasicAuthSchemeOptions, BasicAuthHandler>(
                authenticationScheme, configureOptions);
        }
    }
}
