using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PhoneBook.Api.Data;
using PhoneBook.Api.Middleware;
using PhoneBook.Api.Repositories;
using PhoneBook.Api.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "PhoneBook API", Version = "v1" });
});

var connectionString = Environment.GetEnvironmentVariable("ORACLE_CONNECTION_STRING") 
    ?? builder.Configuration.GetConnectionString("Oracle");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(connectionString));

builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddSingleton<IDeletionLogService, DeletionLogService>();

var corsOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
var allowedOrigins = !string.IsNullOrEmpty(corsOrigins) 
    ? corsOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries)
    : builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database");

var app = builder.Build();

await InitializeDatabaseAsync(app);

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PhoneBook API v1");
    });
}

app.UseCors("AllowFrontend");
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    const int maxRetries = 10;
    const int delayMs = 5000;

    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            await db.Database.EnsureCreatedAsync();
            logger.LogInformation("Database initialized successfully");
            
            await DbSeeder.SeedAsync(db, logger);
            return;
        }
        catch (Exception ex) when (attempt < maxRetries)
        {
            logger.LogWarning("Database not ready (attempt {Attempt}/{MaxRetries}): {Message}",
                attempt, maxRetries, ex.Message);
            await Task.Delay(delayMs);
        }
    }

    throw new InvalidOperationException("Failed to initialize database after maximum retries");
}
