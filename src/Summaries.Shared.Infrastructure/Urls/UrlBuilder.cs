using Microsoft.AspNetCore.Http;

namespace Summaries.Shared.Infrastructure.Urls;

public sealed class UrlBuilder(IHttpContextAccessor httpContextAccessor)
    : IUrlBuilder
{
    public string? ToAbsoluteUrl(string? relativeUrl)
    {
        if (string.IsNullOrEmpty(relativeUrl))
        {
            return relativeUrl;
        }

        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            return relativeUrl;
        }

        var cacheBuster = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{relativeUrl}?v={cacheBuster}";
    }
}