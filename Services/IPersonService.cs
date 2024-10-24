using SGTD_WebApi.Models.Person;

namespace SGTD_WebApi.Services;

public interface IPersonService : IBaseService<PersonRequestParams, PersonDto>;