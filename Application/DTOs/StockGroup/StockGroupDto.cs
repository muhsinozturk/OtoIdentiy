using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StockGroup
{
    public class StockGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string UnitType { get; set; } = null!; // Adet, Litre, Kilo gibi
    }
}
