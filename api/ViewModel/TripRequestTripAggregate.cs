using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.ViewModel
{
    public class TripRequestTripAggregate
    {
        public int Id { get; set; }
        //public string RequestCode { get; set; } = null!;
        public int RequesterId { get; set; }
        public string RequesterName { get; set; } = null!;
        public string? RequesterPhone { get; set; }
        public string FromLocation { get; set; } = null!;
        public string ToLocation { get; set; } = null!;
        //public DateTimeOffset RequestedAt { get; set; }
        //public DateTimeOffset? HandledAt { get; set; }
        //public string Purpose { get; set; } = null!;
        public int TripRequestStatusId { get; set; }
        public string? Description { get; set; }
        //public string? RejectReason { get; set; }
        //public string? CancelReason { get; set; }
        public string TripRequestStatusName { get; set; } = null!;
        public string TripRequestStatusColor { get; set; } = null!;
        public int? TripId { get; set; }
        //public string? TripCode { get; set; } = null!;
        public int? TripStatusId { get; set; }
        public string? TripStatusName { get; set; } = null!;
        public string? TripStatusColor { get; set; } = null!;
        public int? VehicleId { get; set; }
        //public string? VehicleLicensePlate { get; set; } = null!;
        public string? VehicleModelName { get; set; } = null!;
        public string? VehicleBrandName { get; set; } = null!;
        public int? DriverId { get; set; }
        public string? DriverName { get; set; } = null!;
        public string? DriverPhone { get; set; }
        //public string? Notes { get; set; }
        public DateTimeOffset? ActualStartTime { get; set; }
        public DateTimeOffset? ActualEndTime { get; set; }
        //public DateTimeOffset? DispatchTime { get; set; }
        //public DateTimeOffset? ConfirmationTime { get; set; }
        public DateTimeOffset? PickUpTime { get; set; }
        //public DateTimeOffset? CancellationTime { get; set; }
        //public DateTimeOffset? CompletionTime { get; set; }
    }

}