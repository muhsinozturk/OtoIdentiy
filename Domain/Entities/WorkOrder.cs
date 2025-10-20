using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class WorkOrder: BaseAuditableEntity
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public int? EmployeeId { get; set; } // sorumlu
    public DateTime OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public string Description { get; set; }
    public decimal LaborCost { get; set; }

    public virtual Invoice? Invoice { get; set; }
    public virtual Vehicle Vehicle { get; set; }
    public virtual Employee Employee { get; set; }
    public virtual List<WorkOrderPart> Parts { get; set; }

}
