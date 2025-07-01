using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Entidad que gestiona los tokens de autenticación y autorización de los usuarios
/// </summary>
[Table("UserTokens")]
public class UserToken : Base
{
    /// <summary>
    /// Identificador único del usuario propietario del token
    /// </summary>
    public Guid UserGuid { get; set; }
    
    /// <summary>
    /// Token JWT para autenticación del usuario
    /// </summary>
    public string Token { get; set; }
    
    /// <summary>
    /// Token de actualización para renovar la sesión
    /// </summary>
    public string RefreshToken { get; set; }
    
    /// <summary>
    /// Fecha y hora de expiración del token
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// Estado de actividad del token (válido/inválido)
    /// </summary>
    public bool IsActive { get; set; }
}