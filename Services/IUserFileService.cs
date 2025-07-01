using SGTD_WebApi.Models.UserFile;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de archivos de usuarios.
/// Proporciona funcionalidades para cargar, descargar, compartir y gestionar archivos de usuarios.
/// </summary>
public interface IUserFileService
{
    /// <summary>
    /// Obtiene todos los archivos de un usuario específico de forma asíncrona.
    /// </summary>
    /// <param name="userGuid">Identificador único del usuario</param>
    /// <returns>Lista de objetos UserFileDto con los archivos del usuario</returns>
    Task<List<UserFileDto>> GetByUserGuIdAsync(Guid userGuid);
    /// <summary>
    /// Carga múltiples archivos para un usuario de forma asíncrona.
    /// </summary>
    /// <param name="userFiles">Lista de archivos a cargar</param>
    /// <param name="userGuid">Identificador único del usuario</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task UploadFilesAsync(List<IFormFile> userFiles, Guid userGuid);
    /// <summary>
    /// Descarga un archivo específico de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único del archivo</param>
    /// <returns>Objeto UserFileByteDto con los datos del archivo descargado</returns>
    Task<UserFileByteDto> DownloadFileAsync(int id);
    /// <summary>
    /// Descarga múltiples archivos comprimidos en un ZIP de forma asíncrona.
    /// </summary>
    /// <param name="ids">Lista de identificadores de archivos a descargar</param>
    /// <returns>Array de bytes del archivo ZIP conteniendo los archivos solicitados</returns>
    Task<byte[]> DownloadMultipleFilesAsync(List<int> ids);
    /// <summary>
    /// Elimina un archivo específico de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único del archivo a eliminar</param>
    /// <param name="userGuid">Identificador único del usuario propietario</param>
    /// <returns>Mensaje de confirmación de la eliminación</returns>
    Task<string> DeleteFileAsync(int id, Guid userGuid);
    /// <summary>
    /// Elimina múltiples archivos de forma asíncrona.
    /// </summary>
    /// <param name="ids">Lista de identificadores de archivos a eliminar</param>
    /// <param name="userGuid">Identificador único del usuario propietario</param>
    /// <returns>Mensaje de confirmación de la eliminación</returns>
    Task<string> DeleteMultipleFilesAsync(List<int> ids, Guid userGuid);
    /// <summary>
    /// Obtiene la información de compartición de un archivo de forma asíncrona.
    /// </summary>
    /// <param name="fileId">Identificador del archivo</param>
    /// <param name="userGuid">Identificador único del usuario</param>
    /// <returns>Objeto FileShareInfoDto con la información de compartición del archivo</returns>
    Task<FileShareInfoDto> GetFileShareInfoAsync(int fileId, Guid userGuid);
    /// <summary>
    /// Comparte un archivo con otras personas de forma asíncrona.
    /// </summary>
    /// <param name="fileId">Identificador del archivo a compartir</param>
    /// <param name="personIds">Lista de identificadores de personas con quienes compartir</param>
    /// <param name="sharedByUserGuid">Identificador único del usuario que comparte</param>
    /// <returns>Mensaje de confirmación de la compartición</returns>
    Task<string> ShareFileAsync(int fileId, List<int> personIds, Guid sharedByUserGuid);
    /// <summary>
    /// Deja de compartir un archivo con un usuario específico de forma asíncrona.
    /// </summary>
    /// <param name="fileId">Identificador del archivo</param>
    /// <param name="userId">Identificador del usuario con quien se deja de compartir</param>
    /// <param name="userGuid">Identificador único del usuario propietario</param>
    /// <returns>Mensaje de confirmación de la acción</returns>
    Task<string> UnshareFileAsync(int fileId, int userId, Guid userGuid);
    /// <summary>
    /// Obtiene todos los archivos compartidos con un usuario de forma asíncrona.
    /// </summary>
    /// <param name="userGuid">Identificador único del usuario</param>
    /// <returns>Lista de objetos UserFileShareDto con los archivos compartidos con el usuario</returns>
    Task<List<UserFileShareDto>> GetSharedFilesAsync(Guid userGuid);
}