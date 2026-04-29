using Azure.Core;
using HNGTask1.DTO;
using HNGTask1.Essential_Enums;
using HNGTask1.Models;
using HNGTask1.Repository.Interfaces;
using HNGTask1.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public async Task<IResult> Authorize()
        {
            var clientId = _config.GetValue<string>("GitHub:client_id");
            var redirectUri = _config.GetValue<string>("GitHub:redirect_url");
            var (verifier, challenge) = Utility.GeneratePKCE();
            var state = Guid.NewGuid().ToString();
            _cache.Set($"pkce:{state}", verifier, TimeSpan.FromMinutes(10));

            var url = $"https://github.com/login/oauth/authorize" +
                    $"?client_id={clientId}" +
                    $"&redirect_uri={redirectUri}" +
                    $"&scope=repo user" +
                    $"&state={state}" +
                    $"&code_challenge={challenge}" +
                    $"&code_challenge_method=S256";

            return Results.Ok(new { status = "success", url = url});
        }

        public async Task<IResult> AuthorizeCallback(AuthCallbackParameter callbankParam)
        {
            var cacheKey = $"pkce:{callbankParam.state}";
            if(!_cache.TryGetValue(cacheKey, out string verifier))
            {
                return Results.BadRequest(new { status = "error", message = "Invalid or expired PKCE state" });
            }

            var token = await ExchangeCode(callbankParam.code, verifier);
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
            return Results.Ok(new { status = "success",  tokenResponse.access_token, tokenResponse.refresh_token, tokenResponse.token_expiry,  user });

        }

        private async Task<string> ExchangeCode(string code, string codeVerifier)
        {

            var clientId = _config["GitHub:client_id"];
            var redirectUri = _config["GitHub:redirect_url"];
            var clientSecret = _config["GitHub:client_secret"];

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://github.com/login/oauth/access_token"
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
               "https://api.github.com/user"
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
                role = UserEnum.analyst.ToString(),
                is_active = true,
                last_login_at = DateTime.UtcNow
            };

           return await _userRepo.AddUser(newUser);
        }

        public async Task<IResult> Refresh (RefreshTokenDTO token)
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

            return Results.Json(new { status = "success", tokenResponse.access_token, tokenResponse.refresh_token });
        }

        //public async Task<IResult> Logout()
        //{

        //}
    }
}
