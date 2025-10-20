using Application.DTOs.StockGroup;

namespace Application.Abstractions.Services
{
    public interface IStockGroupService
    {
        Task<List<StockGroupDto>> GetAllAsync();
        Task<StockGroupDto?> GetByIdAsync(int id);
        Task<StockGroupDto> CreateAsync(CreateStockGroupDto dto);
        Task UpdateAsync(StockGroupDto dto);
        Task DeleteAsync(int id);
    }
}
