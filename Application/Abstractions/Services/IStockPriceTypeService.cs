using Application.DTOs.StockPrice;
using Application.DTOs.StockPriceType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services
{
    public interface IStockPriceTypeService
    {
        Task<List<StockPriceTypeDto>> GetAllAsync();
        Task<StockPriceTypeDto> CreateAsync(CreateStockPriceTypeDto dto);
        Task UpdateAsync(UpdateStockPriceTypeDto dto);
        Task DeleteAsync(int id);
        Task<StockPriceTypeDto?> GetByIdAsync(int id);
        Task<List<string>> GetRelatedStocksAsync(int priceTypeId);
    }
}
