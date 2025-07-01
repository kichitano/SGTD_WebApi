using SGTD_WebApi.Models.DocumentaryProcess;

namespace SGTD_WebApi.Services;

/// <summary>
/// Interfaz para el servicio de gestión de procesos documentarios.
/// Proporciona operaciones CRUD y funcionalidades avanzadas para la gestión de instancias de procesos documentarios.
/// </summary>
public interface IDocumentaryProcessService
{
    /// <summary>
    /// Crea un nuevo proceso documentario de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos del proceso a crear</param>
    /// <param name="userId">Identificador del usuario que crea el proceso</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task CreateAsync(DocumentaryProcessRequestParams requestParams, int userId);
    /// <summary>
    /// Actualiza los datos de un proceso documentario existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros de solicitud con los datos actualizados del proceso</param>
    /// <param name="userId">Identificador del usuario que actualiza el proceso</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task UpdateAsync(DocumentaryProcessRequestParams requestParams, int userId);
    /// <summary>
    /// Obtiene todas las instancias de procesos documentarios de forma asíncrona.
    /// </summary>
    /// <param name="userId">Identificador del usuario que solicita la información</param>
    /// <returns>Lista de objetos DocumentaryProcessInstanceDto con todas las instancias</returns>
    Task<List<DocumentaryProcessInstanceDto>> GetAllAsync(int userId);
    /// <summary>
    /// Elimina una instancia de proceso documentario por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único de la instancia a eliminar</param>
    /// <param name="userId">Identificador del usuario que elimina la instancia</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task DeleteByIdAsync(int id, int userId);

    /// <summary>
    /// Obtiene todos los procesos documentarios del usuario actual de forma asíncrona.
    /// </summary>
    /// <param name="userId">Identificador del usuario</param>
    /// <returns>Lista de objetos DocumentaryProcessInstanceDto con los procesos del usuario</returns>
    Task<List<DocumentaryProcessInstanceDto>> GetMyProcessesAsync(int userId);
    /// <summary>
    /// Obtiene todos los procesos documentarios pendientes para un usuario de forma asíncrona.
    /// </summary>
    /// <param name="userId">Identificador del usuario</param>
    /// <returns>Lista de objetos DocumentaryProcessInstanceDto con los procesos pendientes</returns>
    Task<List<DocumentaryProcessInstanceDto>> GetPendingProcessesForUserAsync(int userId);
    /// <summary>
    /// Obtiene todos los procesos documentarios disponibles para el área del usuario de forma asíncrona.
    /// </summary>
    /// <param name="userId">Identificador del usuario</param>
    /// <returns>Lista de objetos DocumentaryProcessInstanceDto con los procesos disponibles</returns>
    Task<List<DocumentaryProcessInstanceDto>> GetAvailableProcessesForUserAreaAsync(int userId);
    /// <summary>
    /// Obtiene un proceso documentario específico por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="processId">Identificador del proceso</param>
    /// <param name="userId">Identificador del usuario que solicita la información</param>
    /// <returns>Objeto DocumentaryProcessInstanceDto con los datos del proceso o null si no existe</returns>
    Task<DocumentaryProcessInstanceDto?> GetProcessByIdAsync(int processId, int userId);
    /// <summary>
    /// Crea una nueva instancia de proceso documentario de forma asíncrona.
    /// </summary>
    /// <param name="createDto">Datos para crear la instancia del proceso</param>
    /// <param name="userId">Identificador del usuario que crea la instancia</param>
    /// <returns>Objeto DocumentaryProcessInstanceDto con la instancia creada</returns>
    Task<DocumentaryProcessInstanceDto> CreateProcessAsync(CreateDocumentaryProcessInstanceDto createDto, int userId);
    /// <summary>
    /// Toma un paso de proceso documentario para procesarlo de forma asíncrona.
    /// </summary>
    /// <param name="processStepInstanceId">Identificador de la instancia del paso del proceso</param>
    /// <param name="userId">Identificador del usuario que toma el paso</param>
    /// <returns>Objeto DocumentaryProcessInstanceDto con el proceso actualizado</returns>
    Task<DocumentaryProcessInstanceDto> TakeProcessStepAsync(int processStepInstanceId, int userId);
    /// <summary>
    /// Actualiza un paso de proceso documentario de forma asíncrona.
    /// </summary>
    /// <param name="updateDto">Datos de actualización del paso del proceso</param>
    /// <param name="userId">Identificador del usuario que actualiza el paso</param>
    /// <returns>Objeto DocumentaryProcessInstanceDto con el proceso actualizado</returns>
    Task<DocumentaryProcessInstanceDto> UpdateProcessStepAsync(UpdateProcessStepDto updateDto, int userId);
    /// <summary>
    /// Descarga un documento del proceso documentario de forma asíncrona.
    /// </summary>
    /// <param name="documentId">Identificador del documento</param>
    /// <param name="userId">Identificador del usuario que descarga el documento</param>
    /// <returns>Array de bytes del documento descargado</returns>
    Task<byte[]> DownloadDocumentAsync(int documentId, int userId);
    /// <summary>
    /// Obtiene todas las notificaciones de procesos documentarios de un usuario de forma asíncrona.
    /// </summary>
    /// <param name="userId">Identificador del usuario</param>
    /// <returns>Lista de objetos DocumentaryProcessNotificationDto con las notificaciones</returns>
    Task<List<DocumentaryProcessNotificationDto>> GetUserNotificationsAsync(int userId);
    /// <summary>
    /// Marca una notificación de proceso documentario como leída de forma asíncrona.
    /// </summary>
    /// <param name="notificationId">Identificador de la notificación</param>
    /// <param name="userId">Identificador del usuario que marca la notificación</param>
    /// <returns>Tarea que representa la operación asíncrona</returns>
    Task MarkNotificationAsReadAsync(int notificationId, int userId);
}