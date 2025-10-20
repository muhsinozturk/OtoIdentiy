using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.WorkOrder;

public class CreateWorkOrderDto
{
    public int VehicleId { get; set; }
    public int? EmployeeId { get; set; } // sorumlu
    public DateTime OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
  public decimal LaborCost { get; set; }
    public string Description { get; set; }
  
}
