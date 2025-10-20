using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Depot
{
    public class EditDepotDto
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public string No { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
