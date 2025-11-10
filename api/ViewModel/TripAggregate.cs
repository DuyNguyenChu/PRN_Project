using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.ViewModel
{
    public class TripAggregate
    {
        public int Id { get; set; }
        public string TripCode { get; set; } = null!;
        public int TripStatusId { get; set; }
        public string TripStatusName { get; set; } = null!;
        public string TripStatusColor { get; set; } = null!;
        public int? RequesterId { get; set; }
        public string? RequesterName { get; set; }
        public string? RequesterPhone { get; set; }
        public int VehicleId { get; set; }
        public string VehicleLicensePlate { get; set; } = null!;
        public string VehicleModelName { get; set; } = null!;
        public string VehicleBrandName { get; set; } = null!;
        public int DriverId { get; set; }
        public string DriverName { get; set; } = null!;
        public string? DriverPhone { get; set; }
        public string FromLocation { get; set; } = null!;
        public string? FromLatitude { get; set; }
        public string? FromLongitude { get; set; }
        public string ToLocation { get; set; } = null!;
        public string? ToLatitude { get; set; }
        public string? ToLongitude { get; set; }
        public decimal Distance { get; set; }
        public string Purpose { get; set; } = null!;
        public string? Notes { get; set; }
        public DateTimeOffset ScheduledStartTime { get; set; }
        public DateTimeOffset ScheduledEndTime { get; set; }
        public DateTimeOffset? ActualStartTime { get; set; }
        public DateTimeOffset? ActualEndTime { get; set; }
        public string? CancelReason { get; set; }
        public string? RejectReason { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; } = null!;
        public int ApprovalBy { get; set; }
        public string ApprovalByName { get; set; } = null!;
        public DateTimeOffset CreatedDate { get; set; }
    }

}
