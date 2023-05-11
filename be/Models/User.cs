namespace TaskSystem.Models;

public class User
{
    public string Email { get; }
    public string Token { get; }
    public DateTime? ExpirationDate { get; set; }

    public User(string email, string token)
    {
        Email = email;
        Token = token;
    }

    public User(string email, string token, DateTime expirationDate)
    {
        Email = email;
        Token = token;
        ExpirationDate = expirationDate;
    }


}