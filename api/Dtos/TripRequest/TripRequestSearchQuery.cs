using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Extensions;

namespace api.Dtos.TripRequest
{
    public class TripRequestSearchQuery : SearchQuery
    {
        public int? RequesterId { get; set; }
        public int? TripRequestStatusId { get; set; }
        public int? CancelledByUserId { get; set; }
    }
}
