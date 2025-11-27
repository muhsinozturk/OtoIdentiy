using Application.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    private readonly ApplicationDbContext _context;

    public VehicleRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<Vehicle?> GetWithWorkOrdersAsync(int id)
    {
        return await _context.Vehicles
            .Include(v => v.WorkOrders)
            .ThenInclude(w => w.Parts)
            .FirstOrDefaultAsync(v => v.Id == id);
    }
}
