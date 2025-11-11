using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Extensions;

namespace api.DTParameters
{
    public class TripExpenseFilter : SearchQuery
    {
        public List<int> ExpenseTypeIds { get; set; } = new List<int>();            
        public List<int> StatusIds { get; set; } = new List<int>();        
        public List<int> DriverIds { get; set; } = new List<int>();
        public List<int> VehicleIds { get; set; } = new List<int>();
        public List<int> TripIds { get; set; } = new List<int>();
        public List<string> TripCodes { get; set; } = new List<string>();
        public string? PeriodTime { get; set; }
    }


}
