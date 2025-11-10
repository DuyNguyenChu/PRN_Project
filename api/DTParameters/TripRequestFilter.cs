using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Extensions;

namespace api.DTParameters
{
    public class TripRequestFilter : SearchQuery
    {
        public List<int> TripRequestStatusIds { get; set; } = new List<int>();
        public List<int> TripStatusIds { get; set; } = new List<int>();
        public List<int> RequesterIds { get; set; } = new List<int>();
        public string? PeriodTime { get; set; }
    }
}
