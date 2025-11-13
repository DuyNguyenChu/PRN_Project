using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.TripRequest
{
    public class ApproveTripRequestDto
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        //public DateTimeOffset ScheduledStartTime { get; set; }
        //public DateTimeOffset ScheduledEndTime { get; set; }
        //public string? Notes { get; set; }
        public int? ApprovalBy { get; set; }
    }
}
