namespace TaskSystem.Config;

public class MailSettings
{
    public string FromName { get; set; } = null!;
    public string FromMail { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}