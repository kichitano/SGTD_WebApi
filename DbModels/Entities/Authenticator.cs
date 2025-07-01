using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa un autenticador de dos factores para usuarios del sistema
/// </summary>
[Table("Authenticators")]
public class Authenticator : Base
{
    /// <summary>
    /// Identificador único del usuario asociado al autenticador
    /// </summary>
    public Guid UserGuid { get; set; }

    /// <summary>
    /// Token del autenticador generado para la autenticación de dos factores
    /// </summary>
    public string AuthenticatorToken { get; set; }

    /// <summary>
    /// Clave secreta utilizada para generar códigos de autenticación
    /// </summary>
    public string SecretKey { get; set; }

    /// <summary>
    /// Fecha y hora de expiración del autenticador
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Indica si el autenticador está activo y puede ser utilizado
    /// </summary>
    public bool IsActive { get; set; }
}