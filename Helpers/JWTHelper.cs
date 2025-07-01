using Microsoft.IdentityModel.Tokens;
using SGTD_WebApi.DbModels.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SGTD_WebApi.Helpers;

/// <summary>
/// Proporciona métodos auxiliares para la generación y gestión de tokens JWT (JSON Web Tokens).
/// </summary>
public class JwtHelper
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Inicializa una nueva instancia de la clase JwtHelper.
    /// </summary>
    /// <param name="configuration">La configuración de la aplicación.</param>
    public JwtHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Genera un token JWT válido para el usuario especificado con una duración de 10 minutos.
    /// </summary>
    /// <param name="user">El usuario para el cual generar el token JWT.</param>
    /// <returns>Un token JWT firmado que contiene las claims del usuario.</returns>
    public string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim("userGuid", user.UserGuid.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}