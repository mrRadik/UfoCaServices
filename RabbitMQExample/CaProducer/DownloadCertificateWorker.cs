using AutoMapper;
using CaProducer.HttpClient;
using CaProducer.Models;
using Domain.Models;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQBase.Models;
using Services.Interfaces;

namespace CaProducer;

public interface IDownloadCertificateWorker : IBaseWorker
{
}
public class DownloadCertificateWorker : IDownloadCertificateWorker
{
    private readonly ICertificateService _certsService;
    private readonly ICertificateHttpClient _caHttpClient;
    private readonly IRabbitMqService<CertificateEvent> _rabbitService;
    private readonly IDbLogger<DownloadCertificateWorker> _logger;
    private readonly IMapper _mapper;
    private readonly CaProducerSettings _settings;
    private readonly IProgress<string> _progress;
    
    public DownloadCertificateWorker(ICertificateHttpClient caHttpClient,
        ICertificateService certsService,
        IRabbitMqService<CertificateEvent> rabbitService,
        IDbLogger<DownloadCertificateWorker> logger,
        IOptions<CaProducerSettings> settings,
        IMapper mapper,
        IProgress<string> progress)
    {
        _caHttpClient = caHttpClient;
        _certsService = certsService;
        _rabbitService = rabbitService;
        _logger = logger;
        _mapper = mapper;
        _settings = settings.Value;
        _progress = progress;
    }

    public async Task Start(CancellationToken token)
    {
        _progress.Report("Start");
        var certList = await _caHttpClient.GetCertList(_settings.Page, _settings.Records);

        foreach (var cert in certList.Data)
        {
            var isCertificateExists = await _certsService.IsCertificateExists(cert.CertInfo.Thumbprint);
            
            if (!string.Equals(cert.Status, "active", StringComparison.InvariantCultureIgnoreCase) || isCertificateExists)
            {
                continue;
            }
            
            SendMessage(cert);
            SaveCert(cert);
           
            if (!token.IsCancellationRequested) 
                continue;
            
            _progress.Report("Aborted by user");
            _rabbitService.Dispose();
            return;
        }
        
        _progress.Report("Finish");
        _rabbitService.Dispose();
    }

    private async void SendMessage(CertificateRequestModel cert)
    {
        var rabbitMessageModel = _mapper.Map<CertificateEvent>(cert);
        try
        {
            _rabbitService.SendMessage(rabbitMessageModel);
            _progress.Report($"Message for certificate {cert.CertInfo.Thumbprint} was send successfully");
        }
        catch (Exception exception)
        {
            _progress.Report("Sending rabbit mq message failed");
            await _logger.LogError(exception.Message);
        }
    }

    private async void SaveCert(CertificateRequestModel cert)
    {
        var certEntity = _mapper.Map<CertificateEntity>(cert);
        try
        {
            await _certsService.SaveCertificate(certEntity);
        }
        catch (Exception exception)
        {
            _progress.Report("Saving certificate failed.");
            await _logger.LogError(exception.Message);
        }
    }
}