using SGTD_WebApi.DbModel.Enums;

namespace SGTD_WebApi.DbModel.Entities;

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