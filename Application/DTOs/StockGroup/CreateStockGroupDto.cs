using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StockGroup;

public class CreateStockGroupDto
{
    public string Name { get; set; } = null!;
    public string UnitType { get; set; } = null!;
}
