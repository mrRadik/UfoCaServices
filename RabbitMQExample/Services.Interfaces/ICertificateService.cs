using Domain.Models;

namespace Services.Interfaces;

public interface ICertificateService
{
    Task<CertificateEntity?> GetCertificateByThumbprint(string thumbprint);
    Task SaveCertificate(CertificateEntity cert);
}