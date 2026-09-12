using System.Security.Cryptography;
using System.Text;

namespace Summaries.Infrastructure.Authentication;

internal static class SecurityTokenHasher
{
    public static string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}