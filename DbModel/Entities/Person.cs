using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGTD_WebApi.DbModel.Entities;

[Table("People")]
public class Person : Base
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(100)]
    public string LastName { get; set; }

    [StringLength(20)]
    public string Phone { get; set; }

    [StringLength(2)] 
    public string NationalityCode { get; set; }

    [StringLength(20)]
    public string DocumentNumber { get; set; }

    public bool Gender { get; set; }
}