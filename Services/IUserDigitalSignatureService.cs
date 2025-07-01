namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de firmas digitales de usuarios.
/// Proporciona funcionalidades para cargar y verificar firmas digitales de usuarios.
/// </summary>
public interface IUserDigitalSignatureService
{
    /// <summary>
    /// Carga una firma digital para un usuario de forma asíncrona.
    /// </summary>
    /// <param name="userDigitalSignature">Archivo de la firma digital del usuario</param>
    /// <param name="userGuid">Identificador único del usuario</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task UploadDigitalSignatureAsync(IFormFile userDigitalSignature, Guid userGuid);
    /// <summary>
    /// Verifica si un usuario tiene una firma digital registrada de forma asíncrona.
    /// </summary>
    /// <param name="userGuid">Identificador único del usuario</param>
    /// <returns>True si el usuario tiene una firma digital registrada, False en caso contrario</returns>
    Task<bool> VerifyUserDigitalSignatureAsync(Guid userGuid);
}