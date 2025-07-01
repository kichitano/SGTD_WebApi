namespace SGTD_WebApi.Models.Area;

/// <summary>
/// Representa los datos de transferencia de un área organizacional
/// </summary>
public class AreaDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool Status { get; set; }
    public int? ParentAreaId { get; set; }
}