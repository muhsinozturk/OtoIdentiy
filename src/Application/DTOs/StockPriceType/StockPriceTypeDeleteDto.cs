using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StockPriceType
{
    public class StockPriceTypeDeleteDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Bu fiyat tipine bağlı stoklar
        public List<string> RelatedStocks { get; set; } = new();
    }
}
