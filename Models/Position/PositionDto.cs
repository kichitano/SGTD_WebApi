namespace SGTD_WebApi.Models.Position;

public class PositionDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int AreaId { get; set; }
    public int? ParentPositionId { get; set; }
}