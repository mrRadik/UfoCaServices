using Newtonsoft.Json;

namespace CaProducer.Models;

public class CertListResponseModel
{
    [JsonProperty("ascending")]
    public bool Ascending { get; set; }

    [JsonProperty("issuers")]
    public object? Issuers { get; set; } = default!;

    [JsonProperty("orderBy")]
    public string OrderBy { get; set; } = default!;
    
    [JsonProperty("page")]
    public int Page { get; set; }

    [JsonProperty("recordsOnPage")]
    public int RecordsOnPage { get; set; }

    [JsonProperty("searchString")]
    public string? SearchString { get; set; } = default!;

    [JsonProperty("statuses")]
    public object? Statuses { get; set; } = default!;
}