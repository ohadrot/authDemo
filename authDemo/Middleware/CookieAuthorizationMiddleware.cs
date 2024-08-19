using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class CookieAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public CookieAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the request path starts with "/home"
        if (context.Request.Path.StartsWithSegments("/home"))
        {
            // Check if the "Authorization" cookie exists
            if (context.Request.Cookies.TryGetValue("Authorization", out var token))
            {
                // Check if the token matches the expected token
                if (token == "aaa324dsfvdfrwferw")
                {
                    // Token is valid, proceed to the next middleware
                    await _next(context);
                    return;
                }
            }

            // If the cookie is missing or the token is invalid, return 403 Forbidden
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Access denied. Invalid or missing token.");
        }
        else
        {
            // If the path does not start with "/home", just pass to the next middleware
            await _next(context);
        }
    }
}
