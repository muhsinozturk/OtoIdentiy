using Domain.Entities;

namespace Application.Abstractions.Repositories;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<Vehicle?> GetWithWorkOrdersAsync(int id);
}
