namespace TaskSystem.DTO.User;

public class LoginDto
{
    public string Email { get; }
    public string Token { get; }
    public string? ExpirationDate { get; }

    public LoginDto(string email, string token, DateTime? expirationDate)
    {
        Email = email;
        Token = token;
        ExpirationDate = expirationDate?.ToString("s");
    }
}