using Microsoft.Extensions.Caching.Memory;
using TaskSystem.Config;

namespace TaskSystem.Models;

public class UserCache
{
    private readonly MemoryCache _codeRequestCache = new MemoryCache(new MemoryCacheOptions());
    private readonly MemoryCache _tokenCache = new MemoryCache(new MemoryCacheOptions());
    private readonly Expiration _expiration;


    public UserCache(Expiration expiration)
    {
        _expiration = expiration;
    }

    public UserCache(Expiration expiration, string testingEmail, string testingToken)
    {
        _expiration = expiration;
        _tokenCache.Set(testingToken, new User(testingEmail, testingToken));
    }

    public async Task<string> AddCodeToCodeRequestCacheAsync(string email, string code)
    {
        return await Task.Run(() =>
        {
            var cacheEntryOptions =
                new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(_expiration.RequestCodeTtl));
            return _codeRequestCache.Set(email, code, cacheEntryOptions);
        });
    }

    public async Task RemoveCodeFromCodeRequestCacheAsync(string email)
    {
        await Task.Run(() =>
        {
            _codeRequestCache.Remove(email);
        });
    }

    public async Task<string?> FindCodeInCodeRequestCacheAsync(string email)
    {
        return await Task.Run(() =>
        {
            _codeRequestCache.TryGetValue(email, out string? code);
            return code;
        });
    }

    public async Task<User> AddUserToTokenCacheAsync(User user)
    {
        var cacheEntryOptions =
            new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(_expiration.TokenTtl));
        var createdUser = _tokenCache.Set(user.Token, user, cacheEntryOptions);
        createdUser.ExpirationDate = DateTime.Now.AddMinutes(_expiration.TokenTtl);
        await RemoveCodeFromCodeRequestCacheAsync(user.Email);
        return createdUser;
    }

    public async Task RemoveUserFromTokenCacheAsync(string token)
    {
        await Task.Run(() =>
        {
            _tokenCache.Remove(token);
        });
    }

    public async Task<User?> FindUserInTokenCacheAsync(string token)
    {
        return await Task.Run(() =>
        {
            _tokenCache.TryGetValue(token, out User? user);
            return user;
        });
    }

    public async Task<bool> IsTokenValidAsync(string? authorizationHeader)
    {
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            return false;
        }

        var token = authorizationHeader["Bearer ".Length..];

        var user = await FindUserInTokenCacheAsync(token);

        if (user is null)
        {
            return false;
        }

        return user.Token == token;
    }
}