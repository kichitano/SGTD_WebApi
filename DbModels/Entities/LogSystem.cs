using SGTD_WebApi.DbModels.Enums;

namespace SGTD_WebApi.DbModels.Entities;

public class LogSystem
{
    public int Id { get; set; }
    public string EntityName { get; set; }
    public ActionTypeEnum Action { get; set; }
    public string? PreviousValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid UserId { get; set; }
}