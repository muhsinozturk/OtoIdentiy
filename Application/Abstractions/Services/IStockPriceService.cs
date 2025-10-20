using Application.DTOs.StockPrice;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Abstractions.Services
{
    public interface IStockPriceService
    {
        Task<List<StockPriceDto>> GetByStockIdAsync(int stockId);
        Task<StockPriceDto> CreateAsync(CreateStockPriceDto dto);
        Task AddOrUpdateMultipleAsync(List<CreateStockPriceDto> priceList);
        Task UpdateAsync(StockPriceDto dto);
        Task DeleteAsync(int id);
    }
}
