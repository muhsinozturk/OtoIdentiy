using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repositories;

public interface IActRepository : IRepository<Act>
{
    Task<Act?> GetWithVehiclesAsync(int id);
    Task<List<Act>> GetAllWithVehiclesAsync();
}
