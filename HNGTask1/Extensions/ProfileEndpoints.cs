using HNGTask1.DTO;
using HNGTask1.Essential_Enums;
using HNGTask1.Services;
using Microsoft.AspNetCore.Authorization;

namespace HNGTask1.Extensions
{
    

    public static class ProfileEndpoints
    {
        public static void MapProfileEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/profiles")
                .RequireAuthorization();

            group.MapPost("", async (Request request, ProfileService service) =>
            {
                return await service.AddProfile(request);
            });

            group.MapGet("", async (
                ProfileService service,
                string? gender,
                string? country_Id,
                string? age_group,
                int? min_age,
                int? max_age,
                double? min_gender_probability,
                double? min_country_probability,
                string? sort_by,
                string? order,
                int page = 1,
                int limit = 10) =>
            {
                return await service.GetAllProfiles(
                    gender, country_Id, age_group,
                    min_age, max_age,
                    min_gender_probability, min_country_probability,
                    sort_by, order, page, limit);
            });

            group.MapGet("/search", async (ProfileService service, string q, int page = 1, int limit = 10) =>
            {
                return await service.GetProfilesBySearch(q, page, limit);
            });

            group.MapGet("/{id}", async (Guid id, ProfileService service) =>
            {
                return await service.GetSingleProfile(id);
            });

            group.MapDelete("/{id}", async (Guid id, ProfileService service) =>
            {
                return await service.DeleteProfile(id);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = UserEnum.admin.ToString() });
        }
    }
}
