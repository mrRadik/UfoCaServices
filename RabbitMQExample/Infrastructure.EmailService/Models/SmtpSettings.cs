namespace EmailService.Models;

public class SmtpSettings
{
    public int SmtpDeliveryMethod { get; set; }
    public string PickupDirectoryLocation { get; set; } = default!;
    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool Ssl { get; set; } = default!;
}