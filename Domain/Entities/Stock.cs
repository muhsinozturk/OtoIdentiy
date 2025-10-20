
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Stock: BaseAuditableEntity
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public int StockGroupId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }

  
    public virtual StockGroup StockGroup { get; set; } = null!;
    public virtual Company Company { get; set; }
    public virtual List<Inventory> Inventories { get; set; }
    public virtual List<StockPrice> Prices { get; set; }
}
