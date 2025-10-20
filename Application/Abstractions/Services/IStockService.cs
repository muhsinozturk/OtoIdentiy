using Application.DTOs.Stock;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Abstractions.Services
{
    public interface IStockService
    {
        Task<List<StockDto>> GetAllAsync();
        Task<StockDto?> GetByIdAsync(int id);
        Task<StockDto> CreateAsync(CreateStockDto dto);
        Task UpdateAsync(EditStockDto dto);
        Task DeleteAsync(int id);
        Task<EditStockDto?> GetByIdForEditAsync(int id);
        Task<List<StockDto>> GetByGroupIdAsync(int groupId);
    }
}
