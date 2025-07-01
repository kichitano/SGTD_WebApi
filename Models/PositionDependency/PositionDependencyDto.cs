namespace SGTD_WebApi.Models.PositionDependency;

/// <summary>
/// DTO que representa una dependencia entre posiciones organizacionales.
/// </summary>
public class PositionDependencyDto
{
    public int Id { get; set; }
    public int ParentPositionId { get; set; }
    public int ChildPositionId { get; set; }
}