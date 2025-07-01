using SGTD_WebApi.Models.Authenticator;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de envío de correos electrónicos de autenticación.
/// Proporciona funcionalidades para enviar correos de verificación y autenticación.
/// </summary>
public interface IAuthenticatorEmailService
{
    /// <summary>
    /// Envía un correo electrónico de autenticación de forma asíncrona.
    /// </summary>
    /// <param name="request">Parámetros de solicitud con los datos del correo de autenticación</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task SendAuthenticatorEmailAsync(AuthenticatorEmailRequestParams request);
}