using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModels.Contexts;
using SGTD_WebApi.DbModels.Entities;
using SGTD_WebApi.Models.Person;

namespace SGTD_WebApi.Services.Implementation;

/// <summary>
/// Servicio para gestionar personas del sistema.
/// Proporciona funcionalidades para crear, actualizar, consultar y eliminar personas,
/// con validación de duplicados por nombre, teléfono y número de documento.
/// </summary>
public class PersonService : IPersonService
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de personas.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a las entidades.</param>
    public PersonService(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea una nueva persona de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información de la persona a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ValidationException">Se lanza cuando ya existe una persona con el mismo nombre, teléfono o DNI.</exception>
    public async Task CreateAsync(PersonRequestParams requestParams)
    {
        var personExists = await _context.People
            .Where(q => q.Phone.Equals(requestParams.Phone) || 
                       q.DocumentNumber.Equals(requestParams.DocumentNumber) ||
                       (q.FirstName.Equals(requestParams.FirstName) && q.LastName.Equals(requestParams.LastName)))
            .AnyAsync();

        if (personExists)
        {
            throw new ValidationException("Ya existe una persona con el mismo nombre, número de teléfono o DNI");
        }

        var person = new Person
        {
            FirstName = requestParams.FirstName,
            LastName = requestParams.LastName,
            Phone = requestParams.Phone,
            NationalityCode = requestParams.NationalityCode,
            DocumentNumber = requestParams.DocumentNumber,
            Gender = requestParams.Gender,
            CreatedAt = DateTime.UtcNow
        };
        _context.People.Add(person);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Actualiza una persona existente de forma asíncrona.
    /// </summary>
    /// <param name="requestParams">Parámetros que contienen la información actualizada de la persona.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="ArgumentException">Se lanza cuando el ID de la persona es requerido para la actualización.</exception>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la persona no se encuentra.</exception>
    /// <exception cref="ValidationException">Se lanza cuando ya existe otra persona con el mismo nombre, teléfono o DNI.</exception>
    public async Task UpdateAsync(PersonRequestParams requestParams)
    {
        if (requestParams.Id == 0)
            throw new ArgumentException("Person Id is required for update.");

        var person = await _context.People.FirstOrDefaultAsync(p => p.Id == requestParams.Id);
        if (person == null)
            throw new KeyNotFoundException("Person not found.");

        var duplicateExists = await _context.People
            .Where(q => q.Id != requestParams.Id && 
                       (q.Phone.Equals(requestParams.Phone) || 
                        q.DocumentNumber.Equals(requestParams.DocumentNumber) ||
                        (q.FirstName.Equals(requestParams.FirstName) && q.LastName.Equals(requestParams.LastName))))
            .AnyAsync();

        if (duplicateExists)
        {
            throw new ValidationException("Ya existe otra persona con el mismo nombre, número de teléfono o DNI");
        }

        person.FirstName = requestParams.FirstName;
        person.LastName = requestParams.LastName;
        person.Phone = requestParams.Phone;
        person.NationalityCode = requestParams.NationalityCode;
        person.DocumentNumber = requestParams.DocumentNumber;
        person.Gender = requestParams.Gender;
        person.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene todas las personas de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de objetos DTO que representan todas las personas.</returns>
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

    /// <summary>
    /// Obtiene una persona específica por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único de la persona.</param>
    /// <returns>Un objeto DTO que representa la persona.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la persona no se encuentra.</exception>
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

    /// <summary>
    /// Elimina una persona por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">El identificador único de la persona a eliminar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="KeyNotFoundException">Se lanza cuando la persona no se encuentra.</exception>
    public async Task DeleteByIdAsync(int id)
    {
        var person = await _context.People.FirstOrDefaultAsync(p => p.Id == id);
        if (person == null)
            throw new KeyNotFoundException("Person not found.");

        _context.People.Remove(person);
        await _context.SaveChangesAsync();
    }
}