using Domain.Entities;

namespace Domain.Repositories.Implementations;

public class LogsRepository : GenericRepository<LogEntity>, ILogsRepository
{
    public LogsRepository(DomainContext context) : base(context)
    {
    }
}