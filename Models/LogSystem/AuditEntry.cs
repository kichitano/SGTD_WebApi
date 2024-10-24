using Microsoft.EntityFrameworkCore.ChangeTracking;
using SGTD_WebApi.DbModel.Enums;

namespace SGTD_WebApi.Models.LogSystem;

public class AuditEntry
{
    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }

    public EntityEntry Entry { get; }
    public string EntityName { get; set; }
    public ActionTypeEnum Action { get; set; }
    public Dictionary<string, object?> KeyValues { get; } = new Dictionary<string, object?>();
    public Dictionary<string, object?> OldValues { get; } = new Dictionary<string, object?>();
    public Dictionary<string, object?> NewValues { get; } = new Dictionary<string, object?>();
}