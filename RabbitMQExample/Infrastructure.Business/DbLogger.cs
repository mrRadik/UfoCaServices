using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Business.Enums;
using Services.Interfaces;

namespace Infrastructure.Business;

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