using Application.DTOs.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inventory;

public class CreateInventoryDto
{
    public int DepotId { get; set; }
    public int StockId { get; set; }
    public int StockGroupId { get; set; }
    public decimal Quantity { get; set; }
    public StockDto? StockDto { get; set; }
}
