using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.Person;

namespace SGTD_WebApi.Services.Implementation;

public class PersonService : IPersonService
{
    private readonly DatabaseContext _context;

    public PersonService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(PersonRequestParams requestParams)
    {
        var personExists = await _context.People
            .Where(q => q.Phone.Equals(requestParams.Phone) || q.DocumentNumber.Equals(requestParams.DocumentNumber))
            .AnyAsync();

        if (personExists)
        {
            throw new ValidationException("Ya existe una persona con el mismo número de teléfono o DNI");
        }

        var person = new Person
        {
            FirstName = requestParams.FirstName,
            LastName = requestParams.LastName,
            Phone = requestParams.Phone,
            NationalityCode = requestParams.NationalityCode,
            DocumentNumber = requestParams.DocumentNumber,
            Gender = requestParams.Gender,
            CreatedAt = DateTime.Now
        };
        _context.People.Add(person);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PersonRequestParams requestParams)
    {
        if (requestParams.Id == 0)
            throw new ArgumentException("Person Id is required for update.");

        var person = await _context.People.FirstOrDefaultAsync(p => p.Id == requestParams.Id);
        if (person == null)
            throw new KeyNotFoundException("Person not found.");

        person.FirstName = requestParams.FirstName;
        person.LastName = requestParams.LastName;
        person.Phone = requestParams.Phone;
        person.NationalityCode = requestParams.NationalityCode;
        person.DocumentNumber = requestParams.DocumentNumber;
        person.Gender = requestParams.Gender;
        person.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task<List<PersonDto>> GetAllAsync()
    {
        return await _context.People
            .Select(p => new PersonDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Phone = p.Phone,
                NationalityCode = p.NationalityCode,
                DocumentNumber = p.DocumentNumber,
                Gender = p.Gender
            })
            .ToListAsync();
    }

    public async Task<PersonDto> GetByIdAsync(int id)
    {
        var person = await _context.People.FirstOrDefaultAsync(p => p.Id == id);
        if (person == null)
            throw new KeyNotFoundException("Person not found.");

        return new PersonDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Phone = person.Phone,
            NationalityCode = person.NationalityCode,
            DocumentNumber = person.DocumentNumber,
            Gender = person.Gender
        };
    }

    public async Task DeleteByIdAsync(int id)
    {
        var person = await _context.People.FirstOrDefaultAsync(p => p.Id == id);
        if (person == null)
            throw new KeyNotFoundException("Person not found.");

        _context.People.Remove(person);
        await _context.SaveChangesAsync();
    }
}