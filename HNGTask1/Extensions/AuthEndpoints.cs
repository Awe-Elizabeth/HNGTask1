using HNGTask1.DTO;
using HNGTask1.Services;
using Microsoft.AspNetCore.Authorization;
namespace HNGTask1.Extensions
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/auth");

            group.MapGet("/github", async (AuthService authService) =>
            {
                return await authService.Authorize();
            }).AllowAnonymous();

            group.MapPost("/github/callback", async (AuthService authService, AuthCallbackParameter callbackParameters) =>
            {
                return await authService.AuthorizeCallback(callbackParameters);
            }).AllowAnonymous();

            group.MapPost("/refresh", async (AuthService authService, RefreshTokenDTO token) =>
            {
                return await authService.Refresh(token);
            }).AllowAnonymous();
        }
    }
}
