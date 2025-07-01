using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa las relaciones jerárquicas entre puestos de trabajo
/// </summary>
[Table("PositionDependencies")]
public class PositionDependency : Base
{
    /// <summary>
    /// Identificador del puesto padre (superior jerárquico)
    /// </summary>
    public int ParentPositionId { get; set; }

    [ForeignKey("ParentPositionId")]
    public Position ParentPosition { get; set; }

    /// <summary>
    /// Identificador del puesto hijo (subordinado)
    /// </summary>
    public int ChildPositionId { get; set; }

    [ForeignKey("ChildPositionId")]
    public Position ChildPosition { get; set; }
}