using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inventory
{
    public class DepotInventoryItemDto
    {
        public int StockId { get; set; }
        public string StockName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
    }
}
