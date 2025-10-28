using System;

namespace api.Dtos.VehicleAssignment
{
    public class VehicleAssignmentUpdateDto
    {
        public DateTimeOffset? EndDate { get; set; }
        public int? Status { get; set; }
        public string? Notes { get; set; }
    }
}