using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.DTParameters
{
    public class TripDTParameters : api.Extensions.DTParameters
    {
        public List<int> RequesterIds { get; set; } = new List<int>();
        public List<int> VehicleIds { get; set; } = new List<int>();
        public List<int> DriverIds { get; set; } = new List<int>();
        public List<int> TripStatusIds { get; set; } = new List<int>();
    }

}
