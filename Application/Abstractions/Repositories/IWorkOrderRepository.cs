using Domain.Entities;

namespace Application.Abstractions.Repositories;

public interface IWorkOrderRepository : IRepository<WorkOrder>
{
    Task<WorkOrder?> GetDetailsAsync(int id);
    Task<IEnumerable<WorkOrder>> GetAllWithDetailsAsync();
}
