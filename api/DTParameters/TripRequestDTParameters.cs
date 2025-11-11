using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.DTParameters
{
    public class TripRequestDTParameters : api.Extensions.DTParameters
    {
        public List<int> TripRequestStatusIds { get; set; } = new List<int>();

        public List<int> RequesterIds { get; set; } = new List<int>();

        public List<int> CancellerIds { get; set; } = new List<int>();


    }

}
