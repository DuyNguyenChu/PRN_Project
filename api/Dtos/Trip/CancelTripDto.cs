using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Trip
{
    public class CancelTripDto
    {
        public int TripId { get; set; }
        public int CancelStatusId { get; set; }
        public string CancelReason { get; set; } = null!;
    }
}
