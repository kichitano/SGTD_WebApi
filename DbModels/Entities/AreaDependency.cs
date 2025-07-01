using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

/// <summary>
/// Representa la relación jerárquica entre áreas organizacionales
/// </summary>
[Table("AreaDependencies")]
public class AreaDependency : Base
{
    /// <summary>
    /// Identificador del área padre en la jerarquía
    /// </summary>
    public int ParentAreaId { get; set; }

    /// <summary>
    /// Área padre en la relación jerárquica
    /// </summary>
    [ForeignKey("ParentAreaId")]
    public Area ParentArea { get; set; }

    /// <summary>
    /// Identificador del área hija en la jerarquía
    /// </summary>
    public int ChildAreaId { get; set; }

    /// <summary>
    /// Área hija en la relación jerárquica
    /// </summary>
    [ForeignKey("ChildAreaId")]
    public Area ChildArea { get; set; }
}