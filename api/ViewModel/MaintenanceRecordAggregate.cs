using System;

namespace api.ViewModel
{
    public class MaintenanceRecordAggregate
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string VehicleModelName { get; set; } = null!;
        public string VehicleRegistrationNumber { get; set; } = null!; 
        public int DriverId { get; set; }
        public string DriverName { get; set; } = null!;
        public int? TripId { get; set; }
        public string? TripCode { get; set; } 
        public int Odometer { get; set; }
        public string ServiceType { get; set; } = null!;
        public string ServiceTypeName { get; set; } = null!;
        public decimal? ServiceCost { get; set; } 
        public string ServiceProvider { get; set; } = null!;
        public DateTimeOffset StartTime { get; set; } 
        public DateTimeOffset? EndTime { get; set; } 
        public string? Notes { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; } = null!;
        public string StatusColor { get; set; } = null!;
        public int? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; } 
        public DateTimeOffset? ApprovedDate { get; set; }
        public string? RejectReason { get; set; }
        public DateTimeOffset CreatedDate { get; set; } 
    }
}