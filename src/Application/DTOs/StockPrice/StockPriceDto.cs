using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StockPrice;

public class StockPriceDto
{
    public int Id { get; set; }
    public int StockId { get; set; }
    public string? StockName { get; set; }
    public int StockPriceTypeId { get; set; }
    public string? StockPriceTypeName { get; set; }
    public decimal Price { get; set; }
}
