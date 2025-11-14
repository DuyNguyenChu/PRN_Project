using api.Extensions;
namespace api.DTParameters
{
    public class DriverViolationDTParameters : api.Extensions.DTParameters
    {
        public List<int> VehicleIds { get; set; } = new List<int>();
        public List<int> DriverIds { get; set; } = new List<int>();
        public List<int> TripIds { get; set; } = new List<int>();
        public List<int> VehicleTypeIds { get; set; } = new List<int>();
    }


}