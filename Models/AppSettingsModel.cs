namespace Kant.Identity.Models;

public class AppSettingsModel
{
    public AppSettingsModel(string appName, string host, string password, string mailAddress, string domainName)
    {
        AppName = appName;
        Host = host;
        Password = password;
        MailAddress = mailAddress;
        DomainName = domainName;
    }

    public string AppName { get; set; }

    public string Host { get; set; }

    public string Password { get; set; }

    public string MailAddress { get; set; }

    public string DomainName { get; set; }

}
