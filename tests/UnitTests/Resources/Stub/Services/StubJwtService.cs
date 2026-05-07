using Application.Abstractions.Services;

namespace UnitTests.Resources.Stub.Services;

public class StubJwtService : IJwtService
{
    public string GenerateToken(string login, string role, int userId) => $"{login}-{role}-{userId}";
}
