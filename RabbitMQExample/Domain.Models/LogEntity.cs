namespace Domain.Models;

public class LogEntity : BaseEntity
{
    public string Type { get; set; }= default!;
    public string Application { get; set; } = default!;
    public string Message { get; set; }= default!;
    public DateTime Date { get; set; }
}