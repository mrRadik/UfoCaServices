using Domain.Entities;

namespace Domain.Models;

public class LogEntity : BaseEntity
{
    public string Type { get; set; }
    public string Application { get; set; }
    public string Message { get; set; }
    public DateTime Date { get; set; }
}