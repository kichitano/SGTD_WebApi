namespace SGTD_WebApi.Models.AreaDependency;

public class AreaDependencyDto
{
    public int Id { get; set; }
    public int ParentAreaId { get; set; }
    public int ChildAreaId { get; set; }
}