using api.Extensions;
using api.Helpers;

namespace api.Dtos.FuelLog
{
    public class FuelLogSearchQuery : SearchQuery
    {
        public List<int> VehicleIds { get; set; } = new List<int>();
        public List<int> DriverIds { get; set; } = new List<int>();
        public List<int> TripIds { get; set; } = new List<int>();
        public List<int> StatusIds { get; set; } = new List<int>();
        public string? PeriodTime { get; set; } // Dùng cho CreatedDate
    }
}