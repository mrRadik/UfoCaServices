using CaProducer.Models;

namespace CaProducer.HttpClient;

public interface ICertificateHttpClient
{
    /// <summary>
    /// Скачать полный сертификат
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<string> DownloadCertificate(int id);
    /// <summary>
    /// Получить список сертификатов без тела сертификата, с пагинацией. Если в records указать 0, то можно получить сразу весь список
    /// </summary>
    /// <param name="page">Страница</param>
    /// <param name="records">Количество записей на странице</param>
    /// <returns></returns>
    Task<CertListRequestModel> GetCertList(int page = 1, int records = 10);
}