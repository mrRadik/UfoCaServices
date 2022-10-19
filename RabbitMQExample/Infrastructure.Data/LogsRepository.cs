using Domain.Interfaces;
using Domain.Models;

namespace Infrastructure.Data;

public class LogsRepository : GenericRepository<LogEntity>, ILogsRepository
{
    public LogsRepository(DomainContext context) : base(context)
    {
    }
}