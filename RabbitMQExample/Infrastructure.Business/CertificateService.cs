using Domain.Interfaces;
using Domain.Models;
using Services.Interfaces;

namespace Infrastructure.Business;

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _certificateRepository;
    public CertificateService(ICertificateRepository certificateRepository)
    {
        _certificateRepository = certificateRepository;
    }
    public async Task<CertificateEntity?> GetCertificateByThumbprint(string thumbprint)
    {
       var certs = await _certificateRepository.GetAsync(x => x.Thumbprint.ToLower() == thumbprint.ToLower());

       return certs.ToList().FirstOrDefault();
    }

    public async Task SaveCertificate(CertificateEntity cert)
    {
        await _certificateRepository.CreateAsync(cert);
    }
}