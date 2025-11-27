using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Stock;

public class StockDto
{
    public int Id { get; set; }
    public int? StockGroupId { get; set; }
    public int? CompanyId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string CompanyName { get; set; }
    public string? StockGroupName { get; set; } = null!;  
    public string? StockGroupUnitType { get; set; } = null!;

}
