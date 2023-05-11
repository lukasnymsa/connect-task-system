using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskSystem.Models;

namespace TaskSystem.Filters;

public class UserAuthorizationFilter : IAsyncActionFilter
{
    private readonly UserCache _userCache;

    public UserAuthorizationFilter(UserCache userCache)
    {
        _userCache = userCache;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var tokenHeader) &&
            tokenHeader.Any() && await _userCache.IsTokenValidAsync(tokenHeader[0]))
        {
            await next();
        }
        else
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }
    }
}