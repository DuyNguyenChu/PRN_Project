using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Trip
{
    public class DriverUpdateTripCompleteDto
    {
        public int? TripId { get; set; } = null!;
        //public string TripCode { get; set; } = null!;
        public int EndOdometer { get; set; }
    }
}
