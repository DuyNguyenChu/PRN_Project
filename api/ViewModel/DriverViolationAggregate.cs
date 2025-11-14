using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.ViewModel
{
    public class DriverViolationAggregate
    {
        public int Id { get; set; }
        public int ViolationTypeId { get; set; }
        public string ViolationTypeName { get; set; } = null!;
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = null!;
        public int DriverId { get; set; }
        public string DriverName { get; set; } = null!;
        //public int? TripId { get; set; }
        //public string? TripCode { get; set; } = null!;
        public DateTimeOffset ViolationDate { get; set; }
        public string Location { get; set; } = null!;
        public decimal PenaltyAmount { get; set; }
        //public int? PointsDeducted { get; set; }
        public string ReportedBy { get; set; } = null!;
        public string? Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }

}