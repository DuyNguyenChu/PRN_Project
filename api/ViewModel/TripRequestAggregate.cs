using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.ViewModel
{
    public class TripRequestAggregate
    {
        public int Id { get; set; }
        //public string RequestCode { get; set; } = null!;
        public int RequesterId { get; set; }
        public string FromLocation { get; set; } = null!;
        public decimal? FromLatitude { get; set; } = null!;
        public decimal? FromLongitude { get; set; } = null!;
        public string ToLocation { get; set; } = null!;
        public decimal? ToLatitude { get; set; } = null!;
        public decimal? ToLongitude { get; set; } = null!;
        //public DateTimeOffset RequestedAt { get; set; }
        public DateTimeOffset? ExpectedStartTime { get; set; }
        //public DateTimeOffset? HandledAt { get; set; }
        //public string Purpose { get; set; } = null!;
        public int TripRequestStatusId { get; set; }
        public string? Description { get; set; }
        //public string? RejectReason { get; set; }
        //public string? CancelReason { get; set; }
        public int? CancelledByUserId { get; set; }
        public string? CancelledName { get; set; }
        public string TripRequestStatusName { get; set; } = null!;
        public string TripRequestStatusColor { get; set; } = null!;
        public string RequesterName { get; set; } = null!;
        public DateTimeOffset CreatedDate { get; set; }
    }

}
