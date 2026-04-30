using HNGTask1.DTO;
using HNGTask1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HNGTask1.Extensions
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/auth").RequireRateLimiting("auth-policy");

            group.MapGet("/github", async (AuthService authService, string? client) =>
            {
                return await authService.Authorize(client);
            }).AllowAnonymous();

            group.MapGet("/github/callback", async (AuthService authService, string code, string state, HttpContext ctx) =>
            {
                return await authService.AuthorizeCallback(code, state, ctx);
            }).AllowAnonymous();

            group.MapPost("/refresh", async (AuthService authService, RefreshTokenDTO token, HttpContext ctx) =>
            {
                return await authService.Refresh(token, ctx);
            }).AllowAnonymous();

            app.MapGet("/api/get_user", async (AuthService authService, HttpContext ctx) =>
            {
                return await authService.GetLoggedInUser(ctx);
            }).RequireAuthorization();
        }
    }
}
