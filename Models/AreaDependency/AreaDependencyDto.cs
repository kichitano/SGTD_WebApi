namespace SGTD_WebApi.Models.AreaDependency;

/// <summary>
/// Representa los datos de transferencia de una dependencia entre áreas
/// </summary>
public class AreaDependencyDto
{
    public int Id { get; set; }
    public int ParentAreaId { get; set; }
    public int ChildAreaId { get; set; }
}