using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.TripRequest
{
    public class CancelTripRequestDto
    {
        public int RequesterId { get; set; }
        //public string CancelReason { get; set; } = null!;
        public int? UpdatedBy { get; set; }
    }
}
