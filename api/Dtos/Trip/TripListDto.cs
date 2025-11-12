using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Trip
{
    public class TripListDto
    {
        public int Id { get; set; }
        //public string TripCode { get; set; } = null!;
        public int TripStatusId { get; set; }
        public string TripStatusName { get; set; } = null!;
        public string TripStatusColor { get; set; } = null!;
        public int? TripRequestId { get; set; }
        public int? RequesterId { get; set; }
        public string? RequesterName { get; set; }
        public string? RequesterPhone { get; set; }
        public int VehicleId { get; set; }
        //public string VehicleLicensePlate { get; set; } = null!;
        public string VehicleModelName { get; set; } = null!;
        public int DriverId { get; set; }
        public string DriverName { get; set; } = null!;
        public string? DriverPhone { get; set; }
        public string FromLocation { get; set; } = null!;
        public decimal? FromLatitude { get; set; }
        public decimal? FromLongitude { get; set; }
        public string ToLocation { get; set; } = null!;
        public decimal? ToLatitude { get; set; }
        public decimal? ToLongitude { get; set; }
        //public decimal Distance { get; set; }
        //public string Purpose { get; set; } = null!;
        //public string? Notes { get; set; }
        public int? StartOdometer { get; set; }
        public int? EndOdometer { get; set; }
        //public DateTimeOffset ScheduledStartTime { get; set; }
        //public DateTimeOffset ScheduledEndTime { get; set; }
        public DateTimeOffset? ActualStartTime { get; set; }
        public DateTimeOffset? ActualEndTime { get; set; }
        //public DateTimeOffset? DispatchTime { get; set; }
        //public DateTimeOffset? ConfirmationTime { get; set; }
        public DateTimeOffset? PickUpTime { get; set; }
        //public DateTimeOffset? CancellationTime { get; set; }
        //public string? CancelReason { get; set; }
        public int? CancelledByUserId { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; } = null!;
        public int? ApprovalBy { get; set; }
        //public string ApprovalByName { get; set; } = null!;
        public DateTimeOffset CreatedDate { get; set; }
    }
}
