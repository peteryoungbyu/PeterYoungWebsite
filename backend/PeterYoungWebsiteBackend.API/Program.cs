using Microsoft.EntityFrameworkCore;
using PeterYoungWebsiteBackend.API.Data;

// Load environment variables from .env file before building the app
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = !string.IsNullOrWhiteSpace(builder.Configuration["myConnectionString"])
    ? builder.Configuration["myConnectionString"]
    : builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString) || connectionString == "myConnectionString")
{
    connectionString = Environment.GetEnvironmentVariable("myConnectionString");
}

if (string.IsNullOrWhiteSpace(connectionString) || connectionString == "myConnectionString")
{
    throw new InvalidOperationException("PostgreSQL connection string was not found. Please ensure 'myConnectionString' is defined in your .env file or 'DefaultConnection' in appsettings.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Endpoint to quickly test database connection
app.MapGet("/api/test-db", async (AppDbContext db) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        return canConnect
            ? Results.Ok(new { status = "Success", message = "Connected to PostgreSQL on Raspberry Pi!" })
            : Results.Problem("Database.CanConnectAsync returned false.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
});

app.Run();
