using api.Extensions; 
namespace api.DTParameters
{
    public class FuelLogDTParameters : api.Extensions.DTParameters
    {
        public List<int> VehicleIds { get; set; } = new List<int>();
        public List<int> DriverIds { get; set; } = new List<int>();
        public List<string> FuelTypes { get; set; } = new List<string>();
        public List<int> StatusIds { get; set; } = new List<int>();
        public List<int> TripIds { get; set; } = new List<int>();
    }
}