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
    Task<InventoryDto> CreateAsync(CreateInventoryDto dto);
    Task<List<DepotInventorySummaryDto>> GetDepotSummaryAsync(int depotId);
    Task UpdateAsync(InventoryDto dto);
    Task DeleteAsync(int id);
}