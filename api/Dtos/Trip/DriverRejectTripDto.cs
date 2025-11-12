using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Trip
{
    public class DriverRejectTripDto
    {
        public int? TripId { get; set; } = null!;
        //public string TripCode { get; set; } = null!;
        //public string RejectReason { get; set; } = null!;
    }
}
