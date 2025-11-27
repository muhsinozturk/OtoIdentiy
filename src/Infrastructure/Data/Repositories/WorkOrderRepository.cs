using Application.Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class WorkOrderRepository : Repository<WorkOrder>, IWorkOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkOrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<WorkOrder?> GetDetailsAsync(int id)
        {
            return await _context.WorkOrders
                .Include(w => w.Vehicle)
                .Include(w => w.Employee)
                .Include(w => w.Parts)
                .FirstOrDefaultAsync(w => w.Id == id);
        }
        public async Task<IEnumerable<WorkOrder>> GetAllWithDetailsAsync()
        {
            return await _context.WorkOrders
                .Include(w => w.Vehicle)
                .Include(w => w.Employee)
                .ToListAsync();
        }
    }
}
