
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Vehicle: BaseAuditableEntity
{
    public int Id { get; set; }
    public int ActId { get; set; }
    public string Plate { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string ModelYear { get; set; }
    public string Description { get; set; }

    public virtual Act Act { get; set; }
    public virtual List<WorkOrder> WorkOrders { get; set; }
}
