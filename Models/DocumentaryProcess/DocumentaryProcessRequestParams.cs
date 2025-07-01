using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.DocumentaryProcess;

public class DocumentaryProcessRequestParams
{
    public int? Id { get; set; }

    [Required]
    public int DocumentaryProcessId { get; set; }

    [Required]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    public bool Status { get; set; } = true;
}