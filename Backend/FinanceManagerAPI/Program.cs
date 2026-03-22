using System.Text;
using System.Text.Json.Serialization;
using FinanceManagerAPI.Data;
using FinanceManagerAPI.Extensions;
using FinanceManagerAPI.Middleware;
using FinanceManagerAPI.Repositories;
using FinanceManagerAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ✅ IMPORTANT FOR RAILWAY (PORT BINDING)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

AppContext.SetSwitch("System.Net.DisableIPv6", true);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


    
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ✅ DATABASE (SAFE)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString =
        Environment.GetEnvironmentVariable("DATABASE_URL")
        ?? builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
    {
        Console.WriteLine("⚠️ DATABASE_URL not set. DB features may not work.");
    }
    else
    {
        if (DatabaseConnectionString.TryConvertDatabaseUrlToNpgsql(connectionString, out var npgsql))
        {
            connectionString = npgsql;
        }

        options.UseNpgsql(connectionString, o => o.EnableRetryOnFailure());
    }
});

// ✅ JWT (SAFE VERSION)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? jwtSettings["SecretKey"]
    ?? "default_dev_secret_key_12345";

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ✅ SERVICES
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IIncomeRepository, IncomeRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();

var app = builder.Build();

// ✅ DATABASE MIGRATION (SAFE)
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}
catch (Exception ex)
{
    Console.WriteLine("⚠️ DB migration failed: " + ex.Message);
}

// ✅ MIDDLEWARE
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Frontend");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// ✅ TEST ROUTE (VERY IMPORTANT)
app.MapGet("/", () => "Backend Running 🚀");

app.MapControllers();

app.Run();
