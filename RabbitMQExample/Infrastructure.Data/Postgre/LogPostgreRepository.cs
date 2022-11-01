using Domain.Interfaces;
using Domain.Models;

namespace Infrastructure.Data.Postgre;

public class LogPostgreRepository : GenericPostgreRepository<LogEntity>, ILogsRepository
{
    public LogPostgreRepository(PostgreContext context) : base(context)
    {
    }
}