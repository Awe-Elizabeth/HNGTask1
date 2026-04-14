var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();



app.MapPost("api/profiles", () =>
{

});

app.Run();


