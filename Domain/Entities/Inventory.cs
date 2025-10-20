using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Inventory: BaseAuditableEntity
{
    public int Id { get; set; }
    public int DepotId { get; set; }
    public int StockId { get; set; }
    public decimal Quantity { get; set; }

    public virtual Depot Depot { get; set; }
    public virtual Stock Stock { get; set; }
}