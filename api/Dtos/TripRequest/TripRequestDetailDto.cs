using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.TripRequest
{
    public class TripRequestDetailDto
    {
        public int Id { get; set; }
        //public string RequestCode { get; set; } = null!;
        public int RequesterId { get; set; }
        public string RequesterName { get; set; } = null!;
        public string FromLocation { get; set; } = null!;
        public decimal? FromLatitude { get; set; } = null!;
        public decimal? FromLongitude { get; set; } = null!;
        public string ToLocation { get; set; } = null!;
        public decimal? ToLatitude { get; set; } = null!;
        public decimal? ToLongitude { get; set; } = null!;
        //public decimal Distance { get; set; }
        //public decimal Price { get; set; }
        //public DateTimeOffset RequestedAt { get; set; }
        //public DateTimeOffset? ExpectedStartTime { get; set; }
        //public DateTimeOffset? HandledAt { get; set; }
        //public string Purpose { get; set; } = null!;
        public int TripRequestStatusId { get; set; }
        public string TripRequestStatusName { get; set; } = null!;
        public string TripRequestStatusColor { get; set; } = null!;
        public string? Description { get; set; }
        //public string? RejectReason { get; set; }
        //public string? CancelReason { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public TripRequestTripDto? Trip { get; set; }
    }

    public class TripRequestTripDto
    {
        public int TripId { get; set; }
        //public string TripCode { get; set; } = null!;
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        //public string? Notes { get; set; }
        //public DateTimeOffset ScheduledStartTime { get; set; }
        //public DateTimeOffset ScheduledEndTime { get; set; }
        public DateTimeOffset? ActualStartTime { get; set; }
        public DateTimeOffset? ActualEndTime { get; set; }
        public int? StartOdometer { get; set; }
        public int? EndOdometer { get; set; }
        //public DateTimeOffset? DispatchTime { get; set; }
        //public DateTimeOffset? ConfirmationTime { get; set; }
        public DateTimeOffset? PickUpTime { get; set; }
        //public DateTimeOffset? CancellationTime { get; set; }
        //public string? CancelReason { get; set; }
        //public DateTimeOffset? CompletionTime { get; set; }
        public int TripStatusId { get; set; }
        public string TripStatusName { get; set; } = null!;
        public string TripStatusColor { get; set; } = null!;
    }
}
