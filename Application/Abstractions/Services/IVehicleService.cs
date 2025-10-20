using Application.DTOs;
using Application.DTOs.Vehicle;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Abstractions.Services
{
    public interface IVehicleService
    {
        Task<List<VehicleDto>> GetAllByActIdAsync(int actId);
        Task<VehicleDto?> GetByIdAsync(int id);
        Task<VehicleDto> CreateAsync(CreateVehicleDto dto);
        Task UpdateAsync(VehicleDto dto);
        Task DeleteAsync(int id);
    }
}
