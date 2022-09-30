using System.Security.Cryptography.X509Certificates;
using SystemFacade;

namespace Consumer.CertificateInstaller;

public static class X509Helper
{
    public static void InstallCertificate(string certData, bool isEmulation = false)
    {
        if (isEmulation)
        {
            //Что бы не устанавливать кучу сертификатов на компьютер, эмулируем бурную деятельность.
            Thread.Sleep(100);
            return;
        }
        if (certData.IsNullOrWhiteSpace())
        {
            return;
        }
        
        using var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadWrite);
        var rawCertData = Convert.FromBase64String(certData);
        var cert = new X509Certificate2(rawCertData);
        store.Add(cert);
        store.Close();
    }
}