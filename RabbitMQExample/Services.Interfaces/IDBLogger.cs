namespace Services.Interfaces;

public interface IDbLogger<T>
{
    Task LogError(string message);
    Task LogInfo(string message);
    Task LogWarning(string message);
}