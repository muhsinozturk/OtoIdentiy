using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class StockGroup: BaseAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!; // Motor Yağları, Kaporta, Elektrik, vb.
    public string UnitType { get; set; } = null!; // Adet, Litre, Kilo...

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
