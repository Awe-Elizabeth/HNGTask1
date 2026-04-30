using Azure.Core;
using HNGTask1.DTO;
using HNGTask1.Essential_Enums;
using HNGTask1.Models;
using HNGTask1.Repository.Interfaces;
using HNGTask1.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace HNGTask1.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly IUserRepository _userRepo;
        private readonly ITokenRepository _tokenRepo;
        private readonly ITokenService _tokenService;
        private readonly HttpClient client = new HttpClient();
        public AuthService(IConfiguration config, IMemoryCache cache, IUserRepository repo, ITokenService tokenService, ITokenRepository tokenRepo)
        {
            _config = config;
            _cache = cache;
            _userRepo = repo;
            _tokenService = tokenService;
            _tokenRepo = tokenRepo;
        }
        public async Task<IResult> Authorize(string client = "web")
        {
            var clientId = _config.GetValue<string>("GitHub:client_id");
            var redirectUri = _config.GetValue<string>("GitHub:redirect_url");

            var (verifier, challenge) = Utility.GeneratePKCE();
            var sessionId = Guid.NewGuid().ToString();
            var state = $"{client}:{sessionId}";

            _cache.Set($"pkce:{state}", verifier, TimeSpan.FromMinutes(10));

            var url = $"{_config["GitHub:baseUrl"]}/login/oauth/authorize" +
                    $"?client_id={clientId}" +
                    $"&redirect_uri={redirectUri}" +
                    $"&scope=repo user" +
                    $"&state={state}" +
                    $"&code_challenge={challenge}" +
                    $"&code_challenge_method=S256";

            if (client == "cli")
            {
                return Results.Ok(new
                {
                    status = "success",
                    url,
                    sessionId
                });
            }
            return Results.Ok(new { status = "success", url = url});
        }

        public async Task<IResult> AuthorizeCallback(string code, string state, HttpContext _context)
        {
            var parts = state.Split(':');
            var clientType = parts[0];
            var sessionId = parts[1];


            var cacheKey = $"pkce:{state}";
            if(!_cache.TryGetValue(cacheKey, out string verifier))
            {
                return Results.BadRequest(new { status = "error", message = "Invalid or expired PKCE state" });
            }


            var token = await ExchangeCode(code, verifier);
            _cache.Remove(cacheKey);

            var user = await GetUser(token);
            var tokenResponse = _tokenService.GenerateTokens(user.id.ToString(), user.role);

            var refreshTokenDetails = new RefreshToken()
            {
                token = tokenResponse.refresh_token,
                created_at = DateTime.UtcNow,
                expires_at = DateTime.UtcNow.AddMinutes(5),
                is_valid = true,
                user_id = user.id,
            };

            await _tokenRepo.AddToken(refreshTokenDetails);
            AddCookies(_context, tokenResponse.access_token, tokenResponse.refresh_token);

            if (clientType == "cli")
            {
                _cache.Set($"cli-session:{sessionId}", new
                {
                    tokenResponse.access_token,
                    tokenResponse.refresh_token,
                    tokenResponse.token_expiry,
                    username = user?.name
                }, TimeSpan.FromMinutes(5));

                //return Results.Content("Login successful. Return to CLI.");

                return Results.Ok(new { status = "complete", token = tokenResponse, username = user?.name });
            }
            //return Results.Ok(new { status = "success",  tokenResponse.access_token, tokenResponse.refresh_token, tokenResponse.token_expiry,  user });
            return Results.Redirect($"{_config["frontend_base"]}/dashboard");
        }

        private async Task<string> ExchangeCode(string code, string codeVerifier)
        {

            var clientId = _config["GitHub:client_id"];
            var redirectUri = _config["GitHub:redirect_url"];
            var clientSecret = _config["GitHub:client_secret"];

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_config["GitHub:baseUrl"]}/login/oauth/access_token"
            );

            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["code"] = code,
                ["redirect_uri"] = redirectUri,
                ["code_verifier"] = codeVerifier
            });

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"GitHub token exchange failed: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();

             var res = JsonSerializer.Deserialize<AccessResponse>(json);
            return res.access_token.ToString();
        }

        public async Task<User> GetUser(string accessToken)
        {
            var request = new HttpRequestMessage(
               HttpMethod.Get,
               $"{_config["GitHub:getUserBase"]}/user"
           );

            request.Headers.Authorization = 
                new AuthenticationHeaderValue("Bearer", accessToken);

            request.Headers.UserAgent.ParseAdd("my-app");

            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github+json")
            );

            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<GithubUser>(json);
            var user = await _userRepo.GetUserByGithubId(res.id.ToString());

            if (user != null)
            {
                return user;
            }

            var newUser = new User()
            {
                github_id = res.id.ToString(),
                name = res.name,
                email = res.email,
                avatar_url = res.avatar_url,
                role = UserEnum.admin.ToString(),
                is_active = true,
                last_login_at = DateTime.UtcNow,
                created_at = DateTime.UtcNow
            };

           return await _userRepo.AddUser(newUser);
        }

        public async Task<IResult> GetLoggedInUser(HttpContext ctx)
        {
            var userId =  ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Results.Unauthorized();

            var user = await _userRepo.GetUserById(Guid.Parse(userId));
            return Results.Ok(new { status = "success", message = "successful", user });
        }

        public async Task<IResult> Refresh (RefreshTokenDTO token, HttpContext ctx)
        {
            var refreshToken = await _tokenRepo.GetToken(token.refresh_token);
            if (refreshToken == null) {
                return Results.Json(new { status = "error", message = "invalid token" }, statusCode: 403);
            }

            var user = await _userRepo.GetUserById(refreshToken.user_id);

            var tokenResponse = _tokenService.GenerateTokens(user.id.ToString(), user.role);
            await _tokenRepo.UpdateToken(refreshToken.token);
            var refreshTokenDetails = new RefreshToken()
            {
                token = tokenResponse.refresh_token,
                created_at = DateTime.UtcNow,
                expires_at = DateTime.UtcNow.AddMinutes(5),
                is_valid = true,
                user_id = user.id,
            };
            await _tokenRepo.AddToken(refreshTokenDetails);
            AddCookies(ctx, tokenResponse.access_token, tokenResponse.refresh_token);

            return Results.Json(new { status = "success", tokenResponse.access_token, tokenResponse.refresh_token });
        }

        private void AddCookies(HttpContext _context, string accessToken, string refreshToken)
        {
            _context.Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(3)
            });

            _context.Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
        }

        //public async Task<IResult> Logout()
        //{
            
        //}
    }
}
