
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class StockPriceType: BaseAuditableEntity
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Name { get; set; } // Örn: Liste, Servis
    public string Code { get; set; } // Örn: LIST, SRV

    public virtual Company Company { get; set; }
    public virtual List<StockPrice> StockPrices { get; set; }
}
