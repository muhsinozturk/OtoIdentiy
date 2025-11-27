using Application.DTOs.Depot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Abstractions.Services
{
    public interface IDepotService
    {
        Task<List<DepotDto>> GetAllAsync();
        Task<DepotDto?> GetByIdAsync(int id);
        Task<DepotDto> CreateAsync(CreateDepotDto dto);
        Task UpdateAsync(EditDepotDto dto);
        Task DeleteAsync(int id);
    }
}
