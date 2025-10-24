using System;

namespace api.Dtos.VehicleType
{
    public class VehicleTypeCreateDto
    {
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}