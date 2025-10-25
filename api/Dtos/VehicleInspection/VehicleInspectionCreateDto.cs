using System;

namespace api.Dtos.VehicleInspection
{
    public class VehicleInspectionCreateDto
    {
        public int VehicleId { get; set; }
        public DateTimeOffset InspectionDate { get; set; }
        public int InspectorId { get; set; }
        public string Result { get; set; } = null!;
        public string? Notes { get; set; }
        public int Status { get; set; }
    }
}