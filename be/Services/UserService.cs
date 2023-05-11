using System.Security.Authentication;
using TaskSystem.Models;
using TaskSystem.Config;

namespace TaskSystem.Services;

public class UserService : IUserService
{
    private readonly UserCache _cache;
    private static readonly Random Random = new Random();
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(UserCache cache, IHttpContextAccessor httpContextAccessor)
    {
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> RequestCodeAsync(string email)
    {
        return await _cache.AddCodeToCodeRequestCacheAsync(email, GenerateCode());
    }

    public async Task<User> LoginAsync(string email, string code)
    {
        var cachedCode = await _cache.FindCodeInCodeRequestCacheAsync(email);

        if (cachedCode != code)
        {
            throw new AuthenticationException("Invalid code.");
        }

        var user = new User(email, GenerateToken());

        return await _cache.AddUserToTokenCacheAsync(user);
    }

    public async Task LogoutAsync(User user)
    {
        await _cache.RemoveUserFromTokenCacheAsync(user.Token);
    }

    public async Task<User> GetLoggedUserAsync()
    {
        string? authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"];

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }

        var token = authorizationHeader["Bearer ".Length..];

        var user = await _cache.FindUserInTokenCacheAsync(token);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }

        return user;
    }

    private static string GenerateCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    private static string GenerateToken()
    {
        return Guid.NewGuid().ToString("N");
    }
}