using BusinessFacade.Services;
using BusinessFacade.Services.Implementations;
using CaProducer.HttpClient;
using Domain.Entities;
using SystemFacade.Helpers;

namespace CaProducer;

public interface IDownloadCertificateWorker
{
    Task Start();
}
public class DownloadCertificateWorker : IDownloadCertificateWorker
{
    private static ICertificateHttpClient _caHttpClient = null!;
    private static ICertificateService _certsService = null!;
    private static IRabbitMqService _rabbitService = null!;
    private static IDbLogger<DownloadCertificateWorker> _logger = null!;

    private static readonly ConsoleHelper<DownloadCertificateWorker> _consoleHelper =
        new ConsoleHelper<DownloadCertificateWorker>();

    public DownloadCertificateWorker(ICertificateHttpClient caHttpClient,
        ICertificateService certsService,
        IRabbitMqService rabbitService,
        IDbLogger<DownloadCertificateWorker> logger)
    {
        _caHttpClient = caHttpClient;
        _certsService = certsService;
        _rabbitService = rabbitService;
        _logger = logger;
    }

    public async Task Start()
    {
        _consoleHelper.Info("Start");
        var certList = await _caHttpClient.GetCertList();

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
                _consoleHelper.Info($"Message for certificate {cert.CertInfo.Thumbprint} was send successful");
            }
            catch (Exception exception)
            {
                _consoleHelper.Info("Sending rabbit mq message failed");
                await _logger.LogError(exception.Message);
            }

            try
            {
                await _certsService.SaveCertificate(certEntity);
            }
            catch (Exception exception)
            {
                _consoleHelper.Info("Saving certificate failed.");
                await _logger.LogError(exception.Message);
            }
        }
        _consoleHelper.Info("Finish");
    }
}