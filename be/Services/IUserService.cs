using TaskSystem.Models;

namespace TaskSystem.Services;

public interface IUserService
{
    public Task<string> RequestCodeAsync(string email);

    public Task<User> LoginAsync(string email, string code);

    public Task LogoutAsync(User user);

    public Task<User> GetLoggedUserAsync();
}