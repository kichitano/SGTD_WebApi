using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SGTD_WebApi.Configurations;
using SGTD_WebApi.DbModels.Contexts;
using System.Text;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Conventions.Insert(0, new RoutePrefixConfiguration("api"));
    options.Filters.Add(new AuthorizeFilter());
});

if (builder.Environment.EnvironmentName == "Testing")
{
    builder.Services.AddDbContext<DatabaseContext>(options =>
        options.UseInMemoryDatabase("TestingDb"));
}
else
{
    string? connectionString = GetConnectionString();

    Console.WriteLine($"Final connection string: {MaskPassword(connectionString)}");

    builder.Services.AddDbContext<DatabaseContext>(options =>
        options.UseNpgsql(connectionString));
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins(
                "https://sgtd-client.vercel.app",
                // "http://localhost:4200",
                "https://*.railway.app")            
                .AllowAnyMethod()
                .AllowAnyHeader() // Allow necesary headers
                .AllowCredentials();
        });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
        };
    });

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ServiceConfiguration.Configure(builder.Services);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<DatabaseContext>();

        logger.LogInformation("Iniciando verificación de base de datos...");

        string finalConnectionString = GetConnectionString();
        logger.LogInformation("Using converted connection string: {ConnectionString}", MaskPassword(finalConnectionString));

        // Crear conexión manual para testing
        using var connection = new Npgsql.NpgsqlConnection(finalConnectionString);
        logger.LogInformation("Intentando abrir conexión manual...");
        await connection.OpenAsync();
        logger.LogInformation("Conexión manual exitosa!");

        if (await context.Database.CanConnectAsync())
        {
            logger.LogInformation("Conexión a base de datos exitosa.");

            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Aplicando {pendingMigrations.Count()} migraciones pendientes...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migraciones aplicadas exitosamente.");
            }
            else
            {
                logger.LogInformation("No hay migraciones pendientes.");
            }
        }
        else
        {
            logger.LogWarning("CanConnectAsync() falló.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error durante la migración: {Message}", ex.Message);
        logger.LogWarning("Continuando sin base de datos.");
    }
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.Use(async (context, next) =>
{
    context.Response.Headers.Remove("X-Powered-By");
    await next();
});

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

static string GetConnectionString()
{
    string? databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (!string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgresql://"))
    {
        try
        {
            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':');

            return $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing DATABASE_URL: {ex.Message}");
            throw;
        }
    }

    // Fallback para desarrollo local
    return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
           "Host=localhost;Database=sgtd_db;Username=postgres;Password=tu_password;Port=5432";
}

static string MaskPassword(string? connectionString)
{
    if (string.IsNullOrEmpty(connectionString)) return "null";

    return System.Text.RegularExpressions.Regex.Replace(
        connectionString,
        @"Password=([^;]+)",
        "Password=***"
    );
}

await app.RunAsync();