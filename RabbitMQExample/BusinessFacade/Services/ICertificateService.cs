using Domain.Entities;

namespace BusinessFacade.Services;

public interface ICertificateService
{
    Task<CertificateEntity?> GetCertificateByThumbprint(string thumbprint);
    Task SaveCertificate(CertificateEntity cert);
}