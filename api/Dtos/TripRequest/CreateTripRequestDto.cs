using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.TripRequest
{
    public class CreateTripRequestDto
    {
        public string FromLocation { get; set; } = null!;
        public decimal? FromLatitude { get; set; } = null!;
        public decimal? FromLongitude { get; set; } = null!;
        public string ToLocation { get; set; } = null!;
        public decimal? ToLatitude { get; set; } = null!;
        public decimal? ToLongitude { get; set; } = null!;
        //public decimal Distance { get; set; }
        //public DateTimeOffset? ExpectedStartTime { get; set; }
        //public string Purpose { get; set; } = null!;
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
    }
}
