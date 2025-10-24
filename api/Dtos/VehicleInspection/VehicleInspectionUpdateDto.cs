using System;

namespace api.Dtos.VehicleInspection
{
    public class VehicleInspectionUpdateDto
    {
        public DateTimeOffset? InspectionDate { get; set; }
        public int? InspectorId { get; set; }
        public string? Result { get; set; }
        public string? Notes { get; set; }
        public int? Status { get; set; }
    }
}