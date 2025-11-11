using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.DTParameters
{
    public class TripExpenseDTParameters : api.Extensions.DTParameters
    {
        public List<int> VehicleIds { get; set; } = new List<int>();        
        public List<int> StatusIds { get; set; } = new List<int>();        
        public List<int> ExpenseTypeIds { get; set; } = new List<int>();
        public List<int> DriverIds { get; set; } = new List<int>();
        public List<int> TripIds { get; set; } = new List<int>();

    }

}
