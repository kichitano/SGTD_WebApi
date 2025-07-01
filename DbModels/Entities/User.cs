using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Entidad que representa un usuario del sistema con sus credenciales y configuraciones
/// </summary>
[Table("Users")]
public class User : Base
{
    /// <summary>
    /// Identificador único global del usuario para referencias externas
    /// </summary>
    public Guid UserGuid { get; set; }
    
    /// <summary>
    /// Identificador de la persona asociada al usuario
    /// </summary>
    public int PersonId { get; set; }

    [ForeignKey("PersonId")]
    public Person Person { get; set; }

    /// <summary>
    /// Identificador del cargo o posición del usuario (opcional)
    /// </summary>
    public int? PositionId { get; set; }

    [ForeignKey("PositionId")]
    public Position? Position { get; set; }

    /// <summary>
    /// Dirección de correo electrónico para autenticación
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Email { get; set; }

    /// <summary>
    /// Contraseña encriptada del usuario
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Password { get; set; }

    /// <summary>
    /// Tamaño de almacenamiento asignado al usuario en bytes
    /// </summary>
    [Required]
    public long StorageSize { get; set; }

    /// <summary>
    /// Ruta de la carpeta personal del usuario en el sistema de archivos
    /// </summary>
    [Required]
    public string FolderPath { get; set; }

    /// <summary>
    /// Estado del usuario (activo/inactivo)
    /// </summary>
    public bool Status { get; set; }
}