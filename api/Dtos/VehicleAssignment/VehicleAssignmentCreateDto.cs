using System;
namespace api.Dtos.VehicleAssignment
{
    public class VehicleAssignmentCreateDto
    {
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public DateTimeOffset AssignmentDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public int Status { get; set; }
        public string? Notes { get; set; }
    }
}