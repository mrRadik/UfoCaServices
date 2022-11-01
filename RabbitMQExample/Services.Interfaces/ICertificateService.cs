using Domain.Models;

namespace Services.Interfaces;

public interface ICertificateService
{
    Task<bool> IsCertificateExists(string thumbprint);
    Task SaveCertificate(CertificateEntity cert);
}