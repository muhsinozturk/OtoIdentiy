using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.WorkOrder
{
    public class UpdateWorkOrderDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int? EmployeeId { get; set; }
        public string Description { get; set; }
        public decimal LaborCost { get; set; }
    }
}
