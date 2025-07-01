using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa la información personal de una persona en el sistema
/// </summary>
[Table("Persons")]
public class Person : Base
{
    /// <summary>
    /// Nombre de la persona
    /// </summary>
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; }

    /// <summary>
    /// Apellido de la persona
    /// </summary>
    [Required]
    [StringLength(100)]
    public string LastName { get; set; }

    /// <summary>
    /// Número de teléfono
    /// </summary>
    [StringLength(20)]
    public string Phone { get; set; }

    /// <summary>
    /// Código de nacionalidad (ISO 3166-1 alpha-2)
    /// </summary>
    [StringLength(2)] 
    public string NationalityCode { get; set; }

    /// <summary>
    /// Número de documento de identidad
    /// </summary>
    [StringLength(20)]
    public string DocumentNumber { get; set; }

    /// <summary>
    /// Género de la persona (true = masculino, false = femenino)
    /// </summary>
    public bool Gender { get; set; }
}