using api.Extensions; 
namespace api.DTParameters
{
    public class DriverDTParameters : api.Extensions.DTParameters
    {
        public List<int> ExperienceYears { get; set; } = new List<int>();
        public List<string> LicenseClasses { get; set; } = new List<string>();
        public List<int> DriverStatusIds { get; set; } = new List<int>();
    }

}