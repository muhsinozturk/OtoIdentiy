using Application.DTOs.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inventory;

public class InventoryDto
{
    public int Id { get; set; }
    public int DepotId { get; set; }
    public string? DepotName { get; set; }
    public int StockId { get; set; }
    public string? StockName { get; set; }
    public decimal Quantity { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public StockDto? StockDto { get; set; }

}
