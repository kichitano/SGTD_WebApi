using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SGTD_WebApi.Configurations;
using SGTD_WebApi.DbModels.Contexts;
using System.Text;

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
    string? connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ??
                               Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
                               builder.Configuration.GetConnectionString("DefaultConnection");

    Console.WriteLine($"DATABASE_URL from env: '{Environment.GetEnvironmentVariable("DATABASE_URL")}'");
    Console.WriteLine($"Final connection string: '{connectionString}'");

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
        logger.LogInformation("Connection string: {ConnectionString}",
            Environment.GetEnvironmentVariable("DATABASE_URL")?.Substring(0, 50) + "...");

        // Intentar conexión con timeout específico
        using var connection = context.Database.GetDbConnection();
        connection.ConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

        logger.LogInformation("Intentando abrir conexión...");
        await connection.OpenAsync();
        logger.LogInformation("Conexión abierta exitosamente!");

        // Verificar si la base de datos puede conectarse
        if (await context.Database.CanConnectAsync())
        {
            logger.LogInformation("CanConnectAsync() exitoso.");

            // Aplicar migraciones pendientes
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
        logger.LogError(ex, "Error detallado: {Message}", ex.Message);
        logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);

        if (!app.Environment.IsDevelopment())
        {
            logger.LogWarning("Continuando sin base de datos en producción.");
        }
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

await app.RunAsync();