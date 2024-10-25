namespace SGTD_WebApi.Models.Area;

public class AreaDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool Status { get; set; }
    public int? ParentAreaId { get; set; }
}