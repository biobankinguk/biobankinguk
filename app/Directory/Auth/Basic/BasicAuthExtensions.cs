using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Biobanks.Directory.Auth.Basic
{
    // based on
    // https://joonasw.net/view/creating-auth-scheme-in-aspnet-core-2
    // though that abstracts the credential checking into a service
    // and genericises these extensions to specify the service

    /// <summary>
    /// AuthenticationBuilder Extensions for Basic Auth
    /// </summary>
    public static class BasicAuthExtensions
    {
        /// <summary>
        /// Add Basic Authentication to the ASP.NET Core Authentication Middleware
        /// </summary>
        /// <param name="builder"></param>
        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder)
            => AddBasic(builder, BasicAuthDefaults.AuthenticationScheme, _ => { });

        /// <summary>
        /// Add Basic Authentication to the ASP.NET Core Authentication Middleware
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="authenticationScheme"></param>
        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, string authenticationScheme)
            => AddBasic(builder, authenticationScheme, _ => { });

        /// <summary>
        /// Add Basic Authentication to the ASP.NET Core Authentication Middleware
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, Action<BasicAuthSchemeOptions> configureOptions)
            => AddBasic(builder, BasicAuthDefaults.AuthenticationScheme, configureOptions);

        /// <summary>
        /// Add Basic Authentication to the ASP.NET Core Authentication Middleware
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="authenticationScheme"></param>
        /// <param name="configureOptions"></param>
        public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, string authenticationScheme, Action<BasicAuthSchemeOptions> configureOptions)
        {
            builder.Services.AddSingleton<IPostConfigureOptions<BasicAuthSchemeOptions>, BasicAuthPostConfigureOptions>();

            return builder.AddScheme<BasicAuthSchemeOptions, BasicAuthHandler>(
                authenticationScheme, configureOptions);
        }
    }
}
