using System;

namespace api.Dtos.VehicleInspection
{
    public class VehicleInspectionDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public DateTimeOffset InspectionDate { get; set; }
        public int InspectorId { get; set; }
        public string Result { get; set; } = null!;
        public string? Notes { get; set; }
        public int Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}