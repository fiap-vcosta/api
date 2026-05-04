using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = sha256.ComputeHash(bytes);
        return string.Concat(hashBytes.Select(b => b.ToString("x2")));
    }
}
