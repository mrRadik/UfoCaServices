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
    
    public async Task<bool> IsCertificateExists(string thumbprint)
    {
        return await _certificateRepository.IsItemExistsAsync(thumbprint.ToLower());
    }

    public async Task SaveCertificate(CertificateEntity cert)
    {
        await _certificateRepository.CreateAsync(cert);
    }
}