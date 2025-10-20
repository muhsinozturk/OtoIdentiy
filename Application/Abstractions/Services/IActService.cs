using Application.DTOs;
using Application.DTOs.Act;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Abstractions.Services
{
    public interface IActService
    {
        Task<List<ActDto>> GetAllAsync();
        Task<ActDto?> GetByIdAsync(int id);
        Task<ActDto> CreateAsync(CreateActDto dto);
        Task UpdateAsync(ActDto dto);
        Task DeleteAsync(int id);
        Task<List<ActWithVehiclesDto>> GetAllWithVehiclesAsync();
        Task<ActWithVehiclesDto?> GetWithVehiclesAsync(int id);
    }
}
