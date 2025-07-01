using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Entidad que almacena las firmas digitales de los usuarios para autenticación y validación de documentos
/// </summary>
[Table("UserDigitalSignatures")]
public class UserDigitalSignature : Base
{
    /// <summary>
    /// Identificador único del usuario propietario de la firma digital
    /// </summary>
    public Guid UserGuid { get; set; }

    /// <summary>
    /// Nombre o identificador de la firma digital
    /// </summary>
    [Required]
    public string Name { get; set; }

}