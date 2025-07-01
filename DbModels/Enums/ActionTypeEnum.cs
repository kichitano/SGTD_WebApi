namespace SGTD_WebApi.DbModels.Enums;

/// <summary>
/// Enumeración que define los tipos de acciones que se pueden realizar en el sistema
/// </summary>
public enum ActionTypeEnum
{
    /// <summary>
    /// Acción de crear nuevos registros o entidades
    /// </summary>
    Create,
    
    /// <summary>
    /// Acción de leer o consultar información existente
    /// </summary>
    Read,
    
    /// <summary>
    /// Acción de actualizar o modificar registros existentes
    /// </summary>
    Update,
    
    /// <summary>
    /// Acción de eliminar registros o entidades
    /// </summary>
    Delete
}