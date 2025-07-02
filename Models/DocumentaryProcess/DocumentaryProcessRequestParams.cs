using System.ComponentModel.DataAnnotations;

namespace SGTD_WebApi.Models.DocumentaryProcess;

/// <summary>
/// Parámetros de solicitud para operaciones de procesos documentales
/// </summary>
public class DocumentaryProcessRequestParams
{
    public int? Id { get; set; }
    
    public Guid? UserGuid { get; set; }

    public int DocumentaryProcessId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool Status { get; set; } = true;
}