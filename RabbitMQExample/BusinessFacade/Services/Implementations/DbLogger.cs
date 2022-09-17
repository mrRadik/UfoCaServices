using BusinessFacade.Enums;
using Domain.Entities;
using Domain.Repositories;

namespace BusinessFacade.Services.Implementations;

public interface IDbLogger<T>
{
    Task LogError(string message);
    Task LogInfo(string message);
    Task LogWarning(string message);
}
public class DbLogger<T> : IDbLogger<T>
{
    private readonly ILogsRepository _logsRepository;

    public DbLogger(ILogsRepository logsRepository)
    {
        _logsRepository = logsRepository;
    }
    
    public async Task LogError(string message)
    {
        await Log(LogType.Error, message);
    }

    public async Task LogInfo(string message)
    {
        await Log(LogType.Info, message);
    }

    public async Task LogWarning(string message)
    {
        await Log(LogType.Warning, message);
    }

    private async Task Log(LogType type, string message)
    {
        var log = new LogEntity
        {
            Type = type.ToString(),
            Application = typeof(T).ToString(),
            Message = message,
            Date = DateTime.UtcNow
        };
        await _logsRepository.CreateAsync(log);
    }
}