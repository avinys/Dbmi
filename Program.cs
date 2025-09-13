using System.Reflection;
using System.Text.Json.Serialization;
using BdmiAPI.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using BdmiAPI.Services.Interfaces;
using BdmiAPI.Services;
using BdmiAPI.Repositories.Interfaces;
using BdmiAPI.Repositories;


var builder = WebApplication.CreateBuilder(args);

// --- Configuration ---
var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing 'DefaultConnection' in appsettings.");

// --- Services ---
builder.Services.AddDbContext<AppDb>(options =>
    options.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

builder.Services.AddControllers()
    // avoid reference loop issues when returning entities with navigation props
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAll", p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "bdmI API", Version = "v1" });

    // Include XML comments if you enabled <GenerateDocumentationFile>true</...> in the csproj
    var xmlName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlName);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// --- Apply pending migrations automatically (handy for demo/defense) ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.Migrate();
    // TODO: call your seed method here if you add one.
}

// --- Middleware ---
app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "bdmI API v1");
    c.RoutePrefix = "swagger"; // UI at /swagger
});

app.UseHttpsRedirection(); // optional; keep if you’ll run with https

app.MapControllers();

app.Run();

// NOTE:
// - Ensure you have appsettings.Development.json with ConnectionStrings:DefaultConnection.
// - Packages needed: Pomelo.EntityFrameworkCore.MySql, Microsoft.EntityFrameworkCore.Design, Swashbuckle.AspNetCore.
// - Controllers should use [ApiController] to auto-return 400 on invalid DTOs.
