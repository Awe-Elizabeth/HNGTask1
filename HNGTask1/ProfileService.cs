using HNGTask1.DTO;
using HNGTask1.Essential_Enums;
using HNGTask1.Models;
using HNGTask1.Repository;
using System.Runtime.CompilerServices;
namespace HNGTask1
{
    public class ProfileService
    {
        private readonly IConfiguration _config;
        private readonly IProfileRepository _repo;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(IConfiguration config, IProfileRepository repo, ILogger<ProfileService> logger)
        {
            _config = config;
            _repo = repo;
            _logger = logger;
        }

        public async Task<IResult> AddProfile(Request _request)
        {
            try
            {
                var valRes = ValidateInput(_request.name);
                if(valRes == "400")
                {
                    return Results.BadRequest(new { status = "error", message = "name cannot be empty" });
                }
                else if(valRes == "422")
                {
                    return Results.UnprocessableEntity(new { status = "error", message = "name must be a valid string" });
                }

                var client = new HttpClient();
                var genderizeUrl = _config.GetValue<string>("URLS:genderize");
                var agifyUrl = _config.GetValue<string>("URLS:agify");
                var nationalizeUrl = _config.GetValue<string>("URLS:nationalize");

                var task1 = client.GetFromJsonAsync<Genderize>($"{genderizeUrl}?name={_request.name}");
                var task2 = client.GetFromJsonAsync<Agify>($"{agifyUrl}?name={_request.name}");
                var task3 = client.GetFromJsonAsync<Nationalize>($"{nationalizeUrl}?name={_request.name}");

                await Task.WhenAll(task1, task2, task3);

                var genderResponse = await task1;
                var agifyResponse = await task2;
                var nationalizeResponse = await task3;


                if (genderResponse.Count == 0 || genderResponse.Gender == null)
                {
                    return Results.Json(new { status = "error", message = $"{genderizeUrl} returned an invalid response" }, statusCode: 502);
                }
                else if (agifyResponse.Count == 0)
                {
                    return Results.Json(new { status = "error", message = $"{agifyUrl} returned an invalid response" }, statusCode: 502);
                }
                else if (nationalizeResponse.Country == null)
                {
                    return Results.Json(new { status = "error", message = $"{nationalizeUrl} returned an invalid response" }, statusCode: 502);
                }

                var result = new Profile
                {
                    name = genderResponse.Name,
                    gender = genderResponse.Gender,
                    gender_probability = genderResponse.Probability,
                    sample_size = genderResponse.Count,
                    age = agifyResponse.Age,
                    age_group = agifyResponse.Age > 60 ? "senior" : agifyResponse.Age > 20 ? "adult" : agifyResponse.Age > 13 ? "teenager" : "child",
                    country_id = nationalizeResponse.Country.MaxBy(x => x.Probability).Country_id,
                    country_probability = nationalizeResponse.Country.MaxBy(x => x.Probability).Probability,
                };
                var profile = await _repo.GetByName(result.name);
                if (profile != null) {
                    return Results.Json(new { staus = "success", message = "Profile already exists", data = profile }, statusCode: 200);
                }

                var newProfile = await _repo.AddProfile(result);
                return Results.Json(new { staus = "success", data = newProfile }, statusCode: 201);
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occured while creating prifile:{_request.name}: {ex.Message}" );
                return Results.Json(new { staus = "error", message = "an error occured" }, statusCode:500);
            }
        }

        public async Task<IResult> GetAllProfiles(string? gender, string? country_Id, string? age_roup, int? min_age, int? max_age, double? min_gender_probability, double? min_country_probability, string? sort_by, string? order, int page = 1, int limit = 10) 
        {
            try
            {
                if(!string.IsNullOrEmpty(sort_by) && !Enum.TryParse<SortEnum>(sort_by, true, out _))
                {
                    return Results.Json(new { staus = "error", message = "Invalid query parameters" }, statusCode: 400);
                }
                var profilesResult = await _repo.GetProfiles(gender, country_Id, age_roup, min_age, max_age, min_gender_probability, min_country_probability, sort_by, order, page, limit);
                return Results.Json(new { status = "success", page = profilesResult.Page, limit = profilesResult.Limit, total = profilesResult.TotalCount, data = profilesResult.Profiles });
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occured while getting profiles: {ex.Message}");
                return Results.Json(new { staus = "error", message = "an error occured" }, statusCode: 500);
            }
        }
        public async Task<IResult> GetProfilesBySearch(string query, int page = 1, int limit = 10)
        {
            try
            {
                var queryResult = ProfileSearchParser.Parse(query);

                var profilesResult = await _repo.GetProfiles(queryResult.Gender, queryResult.CountryId, queryResult.AgeGroup, queryResult.MinAge, queryResult.MaxAge, null, null, null, null, page, limit);
                return Results.Json(new { status = "success", page = profilesResult.Page, limit = profilesResult.Limit, total = profilesResult.TotalCount, data = profilesResult.Profiles });
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occured while getting profiles: {ex.Message}");
                return Results.Json(new { staus = "error", message = "an error occured" }, statusCode: 500);
            } 
        }

        public async Task<IResult> GetSingleProfile(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return Results.Json(new { status = "error", message = "Invalid ID" }, statusCode: 400);
                }
                var profile = await _repo.GetOneProfile(id);
                if (profile == null)
                {
                    return Results.Json(new { status = "error", message = "profile does not exist" }, statusCode: 404);
                }
                return Results.Json(new { status = "success", data = profile });
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occured while fetching {id}: {ex.Message}");
                return Results.Json(new { status = "error", message = "an error has occured" }, statusCode: 500);
            }
        }

        public async Task<IResult> DeleteProfile(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return Results.Json(new { status = "error", message = "Invalid ID" }, statusCode: 400);
                }
                var profile = await _repo.DeleteProfile(id);
                if (profile != 1)
                {
                    return Results.Json(new { status = "error", message = "profile does not exist" }, statusCode: 404);
                }
                return Results.Json(new { status = "success"}, statusCode: 204);
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occured while deleting {id}: ", ex.Message);
                return Results.Json(new { status = "error", message = "an error has occured" }, statusCode: 500);
            }
        }

        private string ValidateInput(string input)
        {
            string code;
            if (string.IsNullOrWhiteSpace(input))
                return code = "400";
            if (!input.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                return code = "422";
            return code = "200";
        }
    }
}
