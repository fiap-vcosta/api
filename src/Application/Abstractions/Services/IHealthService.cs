namespace Application.Abstractions.Services;

public interface IHealthService
{
    Task<bool> CheckDatabaseAsync();
}