using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Auth;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationTests.Infrastructure;

public static class ClienteJwtHelper
{
    public const string DefaultKey = "local-jwt-cliente-key-change-me-32chars-min";
    public const string DefaultIssuer = "tech-challenge-cliente";
    public const string DefaultAudience = "tech-challenge-cliente";

    public static string CreateToken(string documento)
    {
        var digits = new string(documento.Where(char.IsDigit).ToArray());
        var signingKey = Encoding.ASCII.GetBytes(DefaultKey);
        var handler = new JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(ClienteJwtClaims.Documento, digits)]),
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = DefaultIssuer,
            Audience = DefaultAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(signingKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        return handler.WriteToken(handler.CreateToken(descriptor));
    }
}
