using System.Collections.Generic;
using Application.DTOs.Vehicle;

namespace Application.DTOs.Act
{
    public class ActWithVehiclesDto : ActDto
    {
        public List<VehicleDto> Vehicles { get; set; } = new();
    }
}
