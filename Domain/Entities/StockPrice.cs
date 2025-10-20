using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

// Stok + Fiyat Tipi → Fiyat kaydı
public class StockPrice: BaseAuditableEntity
{
    public int Id { get; set; }
    public int StockId { get; set; }
    public int StockPriceTypeId { get; set; }
    public decimal Price { get; set; }

    public virtual Stock Stock { get; set; }
    public virtual StockPriceType StockPriceType { get; set; }
}
