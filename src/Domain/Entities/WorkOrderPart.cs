using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class WorkOrderPart: BaseAuditableEntity
{
    public int Id { get; set; }
    public int WorkOrderId { get; set; }
    public int StockId { get; set; }
    public int? StockPriceTypeId { get; set; }
    public decimal Quantity { get; set; }
    public decimal KdvRate { get; set; }  // Örn: 18

    public int DepotId { get; set; }
    public virtual Depot Depot { get; set; }
    public virtual WorkOrder WorkOrder { get; set; }
    public virtual Stock Stock { get; set; }
    public virtual StockPriceType StockPriceType { get; set; }
}

