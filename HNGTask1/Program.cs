using HNGTask1;
using HNGTask1.Data;
using HNGTask1.Data.Seed;
using HNGTask1.DTO;
using HNGTask1.Models;
using HNGTask1.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));
builder.Services.AddCors();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<ISeeder, ProfileSeeder>();
builder.Services.AddScoped<ProfileService>();
// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var seeder = scope.ServiceProvider.GetRequiredService<ISeeder>();
    var conn = config.GetConnectionString("DbConnection");

    if (string.IsNullOrWhiteSpace(conn))
        throw new InvalidOperationException("Connection string 'DbConnection' is missing.");

    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    db.Database.Migrate();
    await seeder.SeedAsync(db);
}

app.UseHttpsRedirection();



app.MapPost("api/profiles", async (Request _request, ProfileService service) =>
{
    return await service.AddProfile(_request);
});

app.MapGet("api/profiles", async (ProfileService service, string ? gender, string ? country_Id, string ? age_group, int ? min_age, int ? max_age,  double ? min_gender_probability, double ? min_country_probability, string? sort_by, string? order, int page = 1, int limit = 10) =>
{
    return await service.GetAllProfiles(gender, country_Id, age_group, min_age, max_age, min_gender_probability, min_country_probability, sort_by, order, page, limit);
});
app.MapGet("api/profiles/search", async ( ProfileService service, string q, int page = 1, int limit = 10) =>
{
    return await service.GetProfilesBySearch(q, page, limit);
});

app.MapGet("api/profiles/{id}", async (Guid id, ProfileService service) =>
{
    return await service.GetSingleProfile(id);
});

app.MapDelete("api/profiles/{id}", async (Guid id, ProfileService service) =>
{
    return await service.DeleteProfile(id);
});

app.Run();


