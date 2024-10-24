using Microsoft.EntityFrameworkCore;
using SGTD_WebApi.DbModel.Context;
using SGTD_WebApi.DbModel.Entities;
using SGTD_WebApi.Models.Area;

namespace SGTD_WebApi.Services.Implementation
{
    public class AreaService : IAreaService
    {
        private readonly DatabaseContext _context;

        public AreaService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(AreaRequestParams requestParams)
        {
            if (await IsAreaNameUniqueAsync(requestParams.Name))
            {
                var area = new Area
                {
                    Name = requestParams.Name,
                    Description = requestParams.Description,
                    Status = requestParams.Status
                };

                _context.Areas.Add(area);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Area name already exists.");
            }
        }

        public async Task UpdateAsync(AreaRequestParams requestParams)
        {
            if (requestParams.Id == null)
                throw new ArgumentNullException(nameof(requestParams.Id), "Area Id is required for update.");

            var area = await _context.Areas.FirstOrDefaultAsync(q => q.Id == requestParams.Id);
            if (area == null)
                throw new KeyNotFoundException("Area not found.");

            if (await IsAreaNameUniqueAsync(requestParams.Name, requestParams.Id))
            {
                area.Name = requestParams.Name;
                area.Description = requestParams.Description;
                area.Status = requestParams.Status;

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Area name already exists.");
            }
        }

        public async Task<List<AreaDto>> GetAllAsync()
        {
            return await _context.Areas
                .Select(a => new AreaDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Status = a.Status
                })
                .ToListAsync();
        }

        public async Task<AreaDto> GetByIdAsync(int id)
        {
            var area = await _context.Areas
                .FirstOrDefaultAsync(a => a.Id == id);

            if (area == null)
                throw new KeyNotFoundException("Area not found.");

            return new AreaDto
            {
                Id = area.Id,
                Name = area.Name,
                Description = area.Description,
                Status = area.Status
            };
        }

        public async Task DeleteByIdAsync(int id)
        {
            var area = await _context.Areas.FirstOrDefaultAsync(q => q.Id == id);
            if (area == null)
                throw new KeyNotFoundException("Area not found.");

            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsAreaNameUniqueAsync(string name, int? excludeAreaId = null)
        {
            var query = _context.Areas
                .Where(a => a.Name == name);

            if (excludeAreaId.HasValue)
                query = query.Where(a => a.Id != excludeAreaId.Value);

            return !await query.AnyAsync();
        }
    }
}