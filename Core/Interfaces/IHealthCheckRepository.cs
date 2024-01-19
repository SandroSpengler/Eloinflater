namespace Core;

public interface IHealthCheckRepository
{
    Task<bool> checkDBConnection();
}
