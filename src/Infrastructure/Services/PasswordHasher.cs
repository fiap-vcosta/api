using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = SHA256.HashData(bytes);
        return string.Concat(hashBytes.Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));
    }
}
