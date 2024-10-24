using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.Person;

public class PersonDto
{
    public int? Id { get; set; }

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