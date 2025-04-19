using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

[Table("UserTokens")]
public class UserToken : Base
{
    public Guid UserGuid { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}