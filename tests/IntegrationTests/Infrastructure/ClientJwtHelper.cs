using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationTests.Infrastructure;

public static class ClientJwtHelper
{
    public const string DefaultKey = "local-jwt-client-key-change-me-32chars-min";
    public const string DefaultIssuer = "tech-challenge-client";
    public const string DefaultAudience = "tech-challenge-client";
    public const string CpfClaim = "cpf";

    public static string CreateToken(
        string cpf,
        string? key = null,
        string? issuer = null,
        string? audience = null,
        TimeSpan? lifetime = null)
    {
        var digits = new string(cpf.Where(char.IsDigit).ToArray());
        var signingKey = Encoding.ASCII.GetBytes(key ?? DefaultKey);
        var handler = new JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(CpfClaim, digits)]),
            Expires = DateTime.UtcNow.Add(lifetime ?? TimeSpan.FromMinutes(30)),
            Issuer = issuer ?? DefaultIssuer,
            Audience = audience ?? DefaultAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(signingKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        return handler.WriteToken(handler.CreateToken(descriptor));
    }
}
