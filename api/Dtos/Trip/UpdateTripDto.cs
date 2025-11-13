using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Trip
{
    public class UpdateTripDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        //public DateTimeOffset ScheduledStartTime { get; set; }
        //public DateTimeOffset? ScheduledEndTime { get; set; }
        public string FromLocation { get; set; } = null!;
        public decimal? FromLatitude { get; set; }
        public decimal? FromLongitude { get; set; }
        public string ToLocation { get; set; } = null!;
        public decimal? ToLatitude { get; set; }
        public decimal? ToLongitude { get; set; }
        //public decimal Distance { get; set; }
        //public string Purpose { get; set; } = null!;
        //public string? Notes { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
