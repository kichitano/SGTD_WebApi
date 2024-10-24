namespace SGTD_WebApi.Services;

public interface IBaseService<in TRequestParams, TDto>
    where TRequestParams : class
    where TDto : class
{
    Task CreateAsync(TRequestParams requestParams);
    Task UpdateAsync(TRequestParams requestParams);
    Task<List<TDto>> GetAllAsync();
    Task<TDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
}