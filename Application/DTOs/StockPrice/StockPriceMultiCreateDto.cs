using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StockPrice;

public class StockPriceMultiCreateDto
{
    public int StockId { get; set; }
    public string? StockName { get; set; }

    // Bu stokla ilgili birden fazla fiyat tipi girilebilecek
    public List<StockPriceInputDto> Prices { get; set; } = new();
}
