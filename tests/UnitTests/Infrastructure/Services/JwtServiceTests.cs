using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace UnitTests.Infrastructure.Services;

public class JwtServiceTests
{
    [Fact]
    public void GenerateToken_IncludesExpectedClaims()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "super-secret-key-1234567890-ABCDEFGH",
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience"
            })
            .Build();

        var service = new JwtService(config);

        // Act
        var tokenString = service.GenerateToken("admin", "Admin", 42);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenString);

        // Assert
        Assert.Equal("test-issuer", token.Issuer);
        Assert.Equal("test-audience", token.Audiences.Single());
        Assert.Contains(token.Claims, c => c.Type == "unique_name" && c.Value == "admin");
        Assert.Contains(token.Claims, c => c.Type == "role" && c.Value == "Admin");
        Assert.Contains(token.Claims, c => c.Type == "userId" && c.Value == "42");
        Assert.True(token.ValidTo > DateTime.UtcNow);
    }
}
