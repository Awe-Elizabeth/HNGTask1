using HNGTask1;
using HNGTask1.Data;
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
builder.Services.AddScoped<ProfileService>();
// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var conn = config.GetConnectionString("DbConnection");

    if (string.IsNullOrWhiteSpace(conn))
        throw new InvalidOperationException("Connection string 'DbConnection' is missing.");

    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();



app.MapPost("api/profiles", async (Request _request, ProfileService service) =>
{
    return await service.AddProfile(_request);
});

app.MapGet("api/profiles", async (string ? gender, string ? country_Id, string ? age_group, ProfileService service) =>
{
    return await service.GetAllProfiles(gender, country_Id, age_group);
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


