using Newtonsoft.Json;

namespace CaProducer.Models;

public class CertListRequestModel : CertListResponseModel
{
    [JsonProperty("total")]
    public int Total { get; set; }
    
    [JsonProperty("data")]
    public IEnumerable<CertificateModel> Data { get; set; }
}
public class CertificateModel
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("date")]
    public long Date { get; set; }

    [JsonProperty("status")] 
    public string Status { get; set; } = default!;

    [JsonProperty("certInfo")] 
    public CertInfo CertInfo { get; set; } = default!;
}

public class CertInfo
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("thumbprint")]
    public string Thumbprint { get; set; } = default!;

    [JsonProperty("issuer")]
    public string Issuer { get; set; } = default!;

    [JsonProperty("subject")]
    public string Subject { get; set; } = default!;

    [JsonProperty("serial")]
    public string Serial { get; set; } = default!;

    [JsonProperty("notBefore")]
    public long NotBefore { get; set; }

    [JsonProperty("notAfter")]
    public long NotAfter { get; set; }
}