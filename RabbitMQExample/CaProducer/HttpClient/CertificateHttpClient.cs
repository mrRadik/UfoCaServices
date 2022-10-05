using System.Net.Http.Headers;
using System.Net.Http.Json;
using BusinessFacade.Services;
using CaProducer.Models;
using Newtonsoft.Json;

namespace CaProducer.HttpClient;

public class CertificateHttpClient : ICertificateHttpClient
{
    private readonly GosUslugiApi _settings;
    private readonly System.Net.Http.HttpClient _client;
    private readonly IDbLogger<CertificateHttpClient> _logger;
    
    public CertificateHttpClient(System.Net.Http.HttpClient client, IDbLogger<CertificateHttpClient> logger)
    {
        _settings = ApplicationSettings.GetInstance()!.GosUslugiApi;
        _client = client;
        _logger = logger;
    }
    
    public async Task<string> DownloadCertificate(int id)
    {
        try
        {
            var response = await ExecuteRequest($"{_settings.DownloadCertUrl}/{id}", HttpMethod.Get);
            var content = await response.ReadAsByteArrayAsync();
            return Convert.ToBase64String(content);
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex.Message);
            return ex.Message;
        }
    }
    
    public async Task<CertListRequestModel> GetCertList(int page = 1, int records = 10)
    {
        var requestBody = new CertListResponseModel
        {
            Ascending = false,
            Issuers = null,
            Page = page,
            RecordsOnPage = records,
            SearchString = null,
            Statuses = null
        };

        try
        {
            var response = await ExecuteRequest($"{_settings.GetCertListUrl}", HttpMethod.Post, requestBody);
            var content = await response.ReadFromJsonAsync<CertListRequestModel>();
            return content ?? new CertListRequestModel();
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex.Message);
            return new CertListRequestModel();
        }
    }
    
    private async Task<HttpContent> ExecuteRequest(string path, HttpMethod method, object? body = null)
    {
        var requestMessage = new HttpRequestMessage(method, path);
        
        if (body != null)
        {
            var content = JsonConvert.SerializeObject(body);
            var buffer = System.Text.Encoding.Default.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            requestMessage.Content = byteContent;
        }

        var response = await _client.SendAsync(requestMessage);
        CheckResponseForErrors(response);
        return response.Content;
    }

    private static async void CheckResponseForErrors(HttpResponseMessage message)
    {
        if (message.IsSuccessStatusCode)
        {
            return;
        }
        
        var responseMessage = await message.Content.ReadAsStringAsync();
        var errorMessage = $"StatusCode: {(int)message.StatusCode}. " +
                           $"Message: {JsonConvert.DeserializeObject<ResponseErrorModel>(responseMessage).Message}";
            
        throw new Exception(errorMessage);
    }
}