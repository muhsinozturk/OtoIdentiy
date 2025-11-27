using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Vehicle;

public class CreateVehicleDto
{
    public int ActId { get; set; }
    public string Plate { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string ModelYear { get; set; }
    public string Description { get; set; }
}
