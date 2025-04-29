using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

[Table("Authenticators")]
public class Authenticator : Base
{
    public Guid UserGuid { get; set; }
    public string AuthenticatorToken { get; set; }
    public string SecretKey { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}