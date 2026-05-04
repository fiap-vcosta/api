namespace Application.Services;

public interface IJwtService
{
    string GenerateToken(string login, string role, int userId);
}