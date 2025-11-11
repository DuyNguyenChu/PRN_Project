using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.TripRequest
{
    public class UpdateTripRequestDto : CreateTripRequestDto
    {
        public int RequesterId { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
