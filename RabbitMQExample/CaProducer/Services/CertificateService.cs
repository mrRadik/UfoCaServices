using CaProducer.HttpClient;

namespace CaProducer.Services;

public class CertificateService
{
    private readonly ICertificateHttpClient _client;
    
    public CertificateService(ICertificateHttpClient client)
    {
        _client = client;
    }
    public async Task<string> GetCertificate(int certId)
    {
        return await _client.DownloadCertificate(certId);
    }
}