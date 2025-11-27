using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StockPriceType;

public class UpdateStockPriceTypeDto
{
    public int Id { get; set; }
    public int? CompanyId { get; set; }
    public string Name { get; set; } // Örn: Liste, Servis
    public string Code { get; set; }
}
