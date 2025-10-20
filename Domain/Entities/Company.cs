using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Company: BaseAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Navigations
    public virtual List<Employee> Employees { get; set; }
    public virtual List<Depot> Depots { get; set; }
    public virtual List<Act> Acts { get; set; }
    public virtual List<Stock> Stocks { get; set; }
    public virtual List<StockPriceType> StockPriceTypes { get; set; }
}
