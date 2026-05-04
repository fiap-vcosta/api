namespace Application.Services;

public interface IHealthService
{
    Task<bool> CheckDatabaseAsync();
}