namespace SGTD_WebApi.Models;

public class DeleteRequestParams
{
    public int Id { get; set; }
}

public class DeleteByGuidRequestParams
{
    public Guid Guid { get; set; }
}