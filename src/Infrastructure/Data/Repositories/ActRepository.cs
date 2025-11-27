using Application.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class ActRepository : Repository<Act>, IActRepository
{
    private readonly ApplicationDbContext _context;

    public ActRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<Act?> GetWithVehiclesAsync(int id)
    {
        return await _context.Acts
            .Include(a => a.Vehicles)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Act>> GetAllWithVehiclesAsync()
    {
        return await _context.Acts
            .Include(a => a.Vehicles)
            .ToListAsync();
    }
}
