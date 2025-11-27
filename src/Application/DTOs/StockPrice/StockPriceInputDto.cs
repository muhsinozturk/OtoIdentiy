using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StockPrice;

public class StockPriceInputDto
{
    public int StockPriceTypeId { get; set; }
    public string? StockPriceTypeName { get; set; }
    public string? Code { get; set; }
    public decimal Price { get; set; }
}
