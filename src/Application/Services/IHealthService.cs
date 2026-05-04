using System.Threading.Tasks;

namespace Application;

public interface IHealthService
{
    Task<bool> CheckDatabaseAsync();
}