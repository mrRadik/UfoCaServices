namespace Domain.Entities;

public class CertificateEntity : BaseEntity
{
    public int CertId { get; set; }
    public string Issuer { get; set; } = default!;
    public long NotAfter { get; set; }
    public long NotBefore { get; set; }
    public string Serial { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string Thumbprint { get; set; } = default!;
    public string Data { get; set; } = default!;
}