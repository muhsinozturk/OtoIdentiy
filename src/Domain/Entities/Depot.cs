
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Depot: BaseAuditableEntity
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string No { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public virtual Company Company { get; set; }
    public virtual List<Inventory> Inventories { get; set; }
}
