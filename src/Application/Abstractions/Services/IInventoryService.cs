using Application.DTOs.Inventory;
using Application.DTOs.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services;

public interface IInventoryService
{
    Task<List<InventoryDto>> GetByDepotIdAsync(int depotId);
    Task<List<StockDto>> GetByGroupIdAsync(int groupId);
    Task CreateAsync(CreateInventoryDto dto);
    Task<List<DepotInventorySummaryDto>> GetDepotSummaryAsync(int depotId);

    Task<List<InventoryDto>> GetByFilterAsync(int depotId, DateTime? startDate, DateTime? endDate, bool? isInput, string? search);
    Task UpdateAsync(InventoryDto dto);
    Task DeleteAsync(int id);
    Task<decimal> GetCurrentStockAsync(int stockId, int depotId);

    Task<InventoryDto?> GetByDepotAndStockAsync(int depotId, int stockId);

}