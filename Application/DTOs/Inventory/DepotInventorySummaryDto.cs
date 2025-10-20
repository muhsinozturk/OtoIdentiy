using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inventory
{
    public class DepotInventorySummaryDto
    {
        public string StockGroupName { get; set; } = string.Empty;
        public string StockName { get; set; } = string.Empty;
        public string? StockModel { get; set; }
        public string? StockBrand { get; set; }
        public decimal TotalQuantity { get; set; }

        public List<DepotInventoryItemDto?> Items { get; set; } = new();
    }

}
