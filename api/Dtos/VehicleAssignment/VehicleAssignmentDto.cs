using System;

namespace api.Dtos.VehicleAssignment
{
    public class VehicleAssignmentDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public DateTimeOffset AssignmentDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public int Status { get; set; }
        public string? Notes { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}