using SGTD_WebApi.Models.Person;

namespace SGTD_WebApi.Services;

public interface IPersonService
{
    Task CreateAsync(PersonRequestParams requestParams);
    Task UpdateAsync(PersonRequestParams requestParams);
    Task<List<PersonDto>> GetAllAsync();
    Task<PersonDto> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
}