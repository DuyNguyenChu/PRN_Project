using System;

namespace api.Dtos.VehicleType
{
    public class VehicleTypeUpdateDto
    {
        public string? Name { get; set; }
        public string? Color { get; set; }
        public string? Description { get; set; }
    }
}