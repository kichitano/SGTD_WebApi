using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModels.Entities;

[Table("UserDigitalSignatures")]
public class UserDigitalSignature : Base
{
    public Guid UserGuid { get; set; }

    [Required]
    public string Name { get; set; }

}