using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Readify.Application.Features.Authentications.V1;
using Readify.Application.Features.Authentications.V1.Models.Response;
using Readify.Application.Features.Authentications.V1.Models.Statics;

namespace Readify.API.Common.Auth
{
    public class AuthMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context)
        {
            var authenticationAppServices = context.RequestServices.GetRequiredService<IAuthenticationAppServices>();

            string? tokenGuid = context.Request.Headers.Authorization.FirstOrDefault();

            Endpoint? endpoint = context.GetEndpoint();

            if (endpoint is null)
            {
                await _next(context);
                return;
            }

            var allowedAnonymous = endpoint.Metadata.Any(m => m.GetType().Equals(typeof(AllowAnonymousAttribute)));

            if (allowedAnonymous)
            {
                await _next(context);
                return;
            }

            if (string.IsNullOrEmpty(tokenGuid) || !Guid.TryParse(tokenGuid, out Guid tokenId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            Result<AuthResponse> auth = await authenticationAppServices.GetTokenByIdAsync(tokenId);

            if (auth.IsFailed || auth.Value.TokenHasExpired)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            AuthManager.Context = auth.Value;            

            await _next.Invoke(context);

            await AuthManager.ClearAsync();
        }
    }
}
