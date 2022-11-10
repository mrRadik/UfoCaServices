using AutoMapper;
using CaProducer.HttpClient;
using CaProducer.Models;
using Domain.Models;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQBase.Models;
using Services.Interfaces;
using SystemFacade;

namespace CaProducer;

public class DownloadCertificateHostedService : BaseHostedService
{
    private readonly ICertificateService _certsService;
    private readonly ICertificateHttpClient _caHttpClient;
    private readonly IRabbitMqService<CertificateEvent> _rabbitService;
    private readonly IDbLogger<DownloadCertificateHostedService> _logger;
    private readonly IMapper _mapper;
    private readonly CaProducerSettings _settings;
    private readonly IProgress<string> _progress;
    
    public DownloadCertificateHostedService(ICertificateHttpClient caHttpClient,
        ICertificateService certsService,
        IRabbitMqService<CertificateEvent> rabbitService,
        IDbLogger<DownloadCertificateHostedService> logger,
        IOptions<CaProducerSettings> settings,
        IMapper mapper,
        IProgress<string> progress) : base(progress)
    {
        _caHttpClient = caHttpClient;
        _certsService = certsService;
        _rabbitService = rabbitService;
        _logger = logger;
        _mapper = mapper;
        _settings = settings.Value;
        _progress = progress;
    }

    protected override async Task DoWork(CancellationToken cancellationToken)
    {
        _progress.Report("Start getting certificates");
        var certList = await _caHttpClient.GetCertList(_settings.Page, _settings.Records);
        _progress.Report($"{certList.Data.Count()} was received");
        
        foreach (var cert in certList.Data)
        {
            _progress.Report($"Start working with {cert.CertInfo.Thumbprint} certificate");
            var isCertificateExists = await _certsService.IsCertificateExists(cert.CertInfo.Thumbprint);
            
            if (!string.Equals(cert.Status, "active", StringComparison.InvariantCultureIgnoreCase) || isCertificateExists)
            {
                _progress.Report($"Certificate {cert.CertInfo.Thumbprint} already exists");
                continue;
            }
            
            SendMessage(cert);
            SaveCert(cert);
           
            if (!cancellationToken.IsCancellationRequested) 
                continue;
            
            await StopAsync(cancellationToken);
            Dispose();
            return;
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _rabbitService.Dispose();
    }

    private async void SendMessage(CertificateRequestModel cert)
    {
        var rabbitMessageModel = _mapper.Map<CertificateEvent>(cert);
        try
        {
            _rabbitService.SendMessage(rabbitMessageModel);
            _progress.Report($"Message for certificate {cert.CertInfo.Thumbprint} was sent successfully");
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
            _progress.Report($"Certificate {certEntity.Thumbprint} was saved succesfully");
        }
        catch (Exception exception)
        {
            _progress.Report("Saving certificate failed.");
            await _logger.LogError(exception.Message);
        }
    }
}