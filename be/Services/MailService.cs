using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using TaskSystem.Config;

namespace TaskSystem.Services;

public class MailService : IMailService
{
    private readonly MailSettings _mailSettings;
    private readonly SmtpClient _client;

    public MailService(IOptions<MailSettings> options)
    {
        _mailSettings = options.Value;
        _client = new SmtpClient();
    }

    public async Task<bool> SendEmail(MimeMessage message)
    {
        await _client.ConnectAsync(_mailSettings.Host, _mailSettings.Port);
        await _client.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);
        message.From.Add(new MailboxAddress(_mailSettings.FromName, _mailSettings.FromMail));
        await using (var stream = new FileStream("Data/email.eml", FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
        {
            await message.WriteToAsync(stream);
        }

        await _client.SendAsync(message);
        await _client.DisconnectAsync(true);
        return true;
    }
}