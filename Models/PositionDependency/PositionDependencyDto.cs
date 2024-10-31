namespace SGTD_WebApi.Models.PositionDependency;

public class PositionDependencyDto
{
    public int Id { get; set; }
    public int ParentPositionId { get; set; }
    public int ChildPositionId { get; set; }
}