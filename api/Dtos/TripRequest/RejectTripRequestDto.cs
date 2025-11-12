using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.TripRequest
{
    public class RejectTripRequestDto
    {
        public int Id { get; set; }
        //public string RejectReason { get; set; } = null!;
        public int? UpdatedBy { get; set; }
    }
}
