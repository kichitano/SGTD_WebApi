using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.Models.Authenticator;

namespace SGTD_WebApi.Services.Implementation;

public class AuthenticatorEmailService : IAuthenticatorEmailService
{

    private readonly string? _apiSendGrid;
    private readonly DatabaseContext _context;
    private readonly IAuthenticatorService _authenticatorService;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public AuthenticatorEmailService(
        IConfiguration configuration, 
        IWebHostEnvironment environment, 
        DatabaseContext context, 
        IAuthenticatorService authenticatorService)
    {
        _configuration = configuration;
        _environment = environment;
        _context = context;
        _authenticatorService = authenticatorService;
        _apiSendGrid = _configuration["SendGrid:ApiKey"];
    }

    public async Task SendAuthenticatorEmailAsync(AuthenticatorEmailRequestParams requestParams)
    {
        var user = await _context.Users
            .Where(q => q.Email.ToLower().Equals(requestParams.Email.ToLower()))
            .Select(q => new
            {
                q.UserGuid,
                FullName = $"{q.Person.FirstName} {q.Person.LastName}",
                q.Email
            })
            .FirstOrDefaultAsync();

        if (user == null)
            throw new InvalidOperationException("User not found.");

        var activationLink = await _authenticatorService.GenerateAuthenticatorKeyAsync(user.UserGuid);

        if (activationLink == null)
            throw new InvalidOperationException("Error creating activation link.");

        if (string.IsNullOrEmpty(_apiSendGrid))
            throw new InvalidOperationException("SendGrid API Key is missing.");

        var client = new SendGridClient(_apiSendGrid);

        var templatePath = Path.Combine(_environment.ContentRootPath, "Templates", "WelcomeEmail.html");
        if (!File.Exists(templatePath))
            throw new FileNotFoundException("Email template not found.", templatePath);

        var htmlContent = await File.ReadAllTextAsync(templatePath);

        htmlContent = htmlContent
            .Replace("{{FullName}}", user.FullName)
            .Replace("{{ActivationLink}}", $"http://localhost:4200/SGTD_Client/activate/{activationLink}");

        var from = new EmailAddress(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:FromName"]);
        var to = new EmailAddress(user.Email, user.FullName);
        var subject = "Bienvenido a SGTD - Activa tu cuenta";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
        var response = await client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to send email. Status code: {response.StatusCode}");
    }
}