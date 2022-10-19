using CaProducer.HttpClient;
using Domain.Models;
using Infrastructure.Interfaces;
using Services.Interfaces;

namespace CaProducer;

public interface IDownloadCertificateWorker : IBaseWorker
{
}
public class DownloadCertificateWorker : IDownloadCertificateWorker
{
    private readonly ICertificateService _certsService;
    private readonly ICertificateHttpClient _caHttpClient;
    private readonly IRabbitMqService _rabbitService;
    private readonly IDbLogger<DownloadCertificateWorker> _logger;
    private readonly IProgress<string> _progress;
    
    public DownloadCertificateWorker(ICertificateHttpClient caHttpClient,
        ICertificateService certsService,
        IRabbitMqService rabbitService,
        IDbLogger<DownloadCertificateWorker> logger,
        IProgress<string> progress)
    {
        _caHttpClient = caHttpClient;
        _certsService = certsService;
        _rabbitService = rabbitService;
        _logger = logger;
        _progress = progress;
    }

    public async Task Start(CancellationToken token)
    {
        var settings = ApplicationSettings.GetInstance()!.CaProducerSettings;
        _progress.Report("Start");
        var certList = await _caHttpClient.GetCertList(settings.Page, settings.Records);

        foreach (var cert in certList.Data)
        {
            var certificateByThumbprint = await _certsService.GetCertificateByThumbprint(cert.CertInfo.Thumbprint);
            if (!string.Equals(cert.Status, "active", StringComparison.InvariantCultureIgnoreCase)
                || certificateByThumbprint != null)
            {
                continue;
            }

            var certData = await _caHttpClient.DownloadCertificate(cert.Id);
            var certEntity = new CertificateEntity
            {
                Issuer = cert.CertInfo.Issuer,
                NotAfter = cert.CertInfo.NotAfter,
                NotBefore = cert.CertInfo.NotBefore,
                Serial = cert.CertInfo.Serial,
                Subject = cert.CertInfo.Subject,
                Thumbprint = cert.CertInfo.Thumbprint,
                CertId = cert.Id,
                Data = certData
            };
            try
            {
                _rabbitService.SendMessage(certEntity);
                _progress.Report($"Message for certificate {cert.CertInfo.Thumbprint} was send successfully");
            }
            catch (Exception exception)
            {
                _progress.Report("Sending rabbit mq message failed");
                await _logger.LogError(exception.Message);
            }

            try
            {
                await _certsService.SaveCertificate(certEntity);
            }
            catch (Exception exception)
            {
                _progress.Report("Saving certificate failed.");
                await _logger.LogError(exception.Message);
            }
            if (!token.IsCancellationRequested) 
                continue;
            
            _progress.Report("Aborted by user");
            _rabbitService.Dispose();
            return;
        }
        
        _progress.Report("Finish");
        _rabbitService.Dispose();
    }
}