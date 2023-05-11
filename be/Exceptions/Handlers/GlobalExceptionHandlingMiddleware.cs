using System.Net;
using System.Security.Authentication;
using Newtonsoft.Json;
using TaskSystem.Exceptions.Errors;
using YouTrackSharp.Generated;

namespace TaskSystem.Exceptions.Handlers;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (YouTrackErrorException e)
        {
            await HandleExceptionAsync(
                context,
                e.Response,
                (HttpStatusCode)e.StatusCode
            );
        }
        catch (UnauthorizedAccessException e)
        {
            await HandleExceptionAsync(
                context,
                e.Message,
                HttpStatusCode.Unauthorized
            );
        }
        catch (UnprocessableDataException e)
        {
            await HandleExceptionAsync(
                context,
                e.Message,
                HttpStatusCode.UnprocessableEntity
            );
        }
        catch (AuthenticationException e)
        {
            await HandleExceptionAsync(
                context,
                e.Message,
                HttpStatusCode.Unauthorized
            );
        }
        catch (InvalidDataException e)
        {
            await HandleExceptionAsync(
                context,
                e.Message,
                HttpStatusCode.BadRequest
            );
        }
        catch (EntityNotFoundException e)
        {
            await HandleExceptionAsync(
                context,
                e.Message,
                HttpStatusCode.NotFound
            );
        }
        catch (Exception)
        {
            await HandleExceptionAsync(
                context,
                "Internal Server Error",
                HttpStatusCode.InternalServerError
            );
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, string errorMessage, HttpStatusCode statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var error = new GlobalError
        {
            Message = errorMessage
        };

        return context.Response.WriteAsync(JsonConvert.SerializeObject(error));
    }
}