using System;

namespace api.Dtos.VehicleAccident
{
    public class VehicleAccidentDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public DateTimeOffset AccidentDate { get; set; }
        public string Location { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal? DamageCost { get; set; }
        public int Status { get; set; }
        public string? RejectReason { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTimeOffset? ApprovedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public int? UpdatedBy { get; set; }    
    }
}