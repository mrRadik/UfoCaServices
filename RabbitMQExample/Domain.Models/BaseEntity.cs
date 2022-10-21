using System.Text.Json.Serialization;

namespace Domain.Models;

public class BaseEntity
{
    [JsonIgnore]
    public int Id { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
}