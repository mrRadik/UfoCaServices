using AutoMapper;
using CaProducer.HttpClient;
using CaProducer.Models;

namespace CaProducer.Profiles.CertificateEvent;

public class CertificateDataMappingAction : IMappingAction<CertificateRequestModel, RabbitMQBase.Models.CertificateEvent>
{
    private readonly ICertificateHttpClient _caHttpClient;

    public CertificateDataMappingAction(ICertificateHttpClient caHttpClient)
    {
        _caHttpClient = caHttpClient;
    }
    
    public async void Process(CertificateRequestModel source, RabbitMQBase.Models.CertificateEvent destination, ResolutionContext context)
    {
        destination.Data = await _caHttpClient.DownloadCertificate(source.Id);
    }
}