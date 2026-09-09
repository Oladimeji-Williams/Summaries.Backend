using System.Text;
using AspNet.Security.OAuth.LinkedIn;
using AspNet.Security.OAuth.Twitter;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Summaries.Infrastructure.Authentication;

internal static class AuthenticationConfiguration
{
    public static IServiceCollection AddAuthenticationConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions =
            configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT configuration is missing.");

        var externalAuth =
            configuration.GetSection(ExternalAuthOptions.SectionName).Get<ExternalAuthOptions>()
            ?? new ExternalAuthOptions();

        var authBuilder = services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            })
            .AddCookie(IdentityConstants.ExternalScheme, options =>
            {
                options.Cookie.Name = ".Summaries.External";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            });

        if (!string.IsNullOrEmpty(externalAuth.Google.ClientId))
        {
            authBuilder.AddGoogle("Google", options =>
            {
                options.ClientId = externalAuth.Google.ClientId;
                options.ClientSecret = externalAuth.Google.ClientSecret;
                options.SignInScheme = IdentityConstants.ExternalScheme;
            });
        }

        if (!string.IsNullOrEmpty(externalAuth.Microsoft.ClientId))
        {
            authBuilder.AddMicrosoftAccount("Microsoft", options =>
            {
                options.ClientId = externalAuth.Microsoft.ClientId;
                options.ClientSecret = externalAuth.Microsoft.ClientSecret;
                options.SignInScheme = IdentityConstants.ExternalScheme;
            });
        }

        if (!string.IsNullOrEmpty(externalAuth.Facebook.ClientId))
        {
            authBuilder.AddFacebook("Facebook", options =>
            {
                options.AppId = externalAuth.Facebook.ClientId;
                options.AppSecret = externalAuth.Facebook.ClientSecret;
                options.SignInScheme = IdentityConstants.ExternalScheme;
            });
        }

        if (!string.IsNullOrEmpty(externalAuth.LinkedIn.ClientId))
        {
            authBuilder.AddLinkedIn("LinkedIn", options =>
            {
                options.ClientId = externalAuth.LinkedIn.ClientId;
                options.ClientSecret = externalAuth.LinkedIn.ClientSecret;
                options.SignInScheme = IdentityConstants.ExternalScheme;
            });
        }

        if (!string.IsNullOrEmpty(externalAuth.Twitter.ClientId))
        {
            authBuilder.AddTwitter("Twitter", options =>
            {
                options.ClientId = externalAuth.Twitter.ClientId;
                options.ClientSecret = externalAuth.Twitter.ClientSecret;
                options.SignInScheme = IdentityConstants.ExternalScheme;
            });
        }
        services.AddAuthorization();
        return services;
    }
}