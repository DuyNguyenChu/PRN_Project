using System;

namespace api.ViewModel
{
    public class FuelLogAggregate
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
        public string FuelType { get; set; } = null!;
        public string FuelTypeName { get; set; } = null!;
        public decimal? UnitPrice { get; set; }
        public decimal? Quantity { get; set; }
        public decimal TotalCost { get; set; }
        public string GasStation { get; set; } = null!; 
        public string? Notes { get; set; }
        public int Status { get; set; }
        public string StatusColor { get; set; } = null!;
        public string StatusName { get; set; } = null!;
        public int? ApprovedBy { get; set; } 
        public string? ApprovedByName { get; set; }
        public DateTimeOffset? ApprovedDate { get; set; } 
        public string? RejectReason { get; set; }
        public DateTimeOffset CreatedDate { get; set; } 
    }
}