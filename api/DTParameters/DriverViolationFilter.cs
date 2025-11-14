using api.Extensions;
namespace api.DTParameters
{
    public class DriverViolationFilter : SearchQuery
    {
        public List<int> ViolationTypeIds { get; set; } = new List<int>();
        public List<int> DriverIds { get; set; } = new List<int>();
        public List<int> VehicleIds { get; set; } = new List<int>();
        public List<int> TripIds { get; set; } = new List<int>();
        public List<string> TripCodes { get; set; } = new List<string>();
    }

}