using AutoMapper;
using CaProducer.HttpClient;
using CaProducer.Models;

namespace CaProducer.Profiles.CertificateEntity;

public class CertificateEntityDataMappingAction : IMappingAction<CertificateRequestModel, Domain.Models.CertificateEntity>
{
    private readonly ICertificateHttpClient _caHttpClient;

    public CertificateEntityDataMappingAction(ICertificateHttpClient caHttpClient)
    {
        _caHttpClient = caHttpClient;
    }
    
    public async void Process(CertificateRequestModel source, Domain.Models.CertificateEntity destination, ResolutionContext context)
    {
        destination.Data = await _caHttpClient.DownloadCertificate(source.Id);
    }
}