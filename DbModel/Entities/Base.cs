using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.DbModel.Entities;

public abstract class Base
{
    [Key]
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}