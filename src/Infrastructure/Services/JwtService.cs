using Application.Services;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly string _key = configuration["Jwt:Key"] ?? "default-key";
    private readonly string _issuer = configuration["Jwt:Issuer"] ?? "default-issuer";
    private readonly string _audience = configuration["Jwt:Audience"] ?? "default-audience";

    public string GenerateToken(string login, string role, int userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, login),
                new Claim(ClaimTypes.Role, role),
                new Claim("userId", userId.ToString())
            ]),
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}