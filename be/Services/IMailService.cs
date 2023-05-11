using MimeKit;

namespace TaskSystem.Services;

public interface IMailService
{
    Task<bool> SendEmail(MimeMessage message);
}