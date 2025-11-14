using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.ViewModel
{
    public class DriverAggregate
    {
        public int Id { get; set; }
        public int? ExperienceYears { get; set; }
        public string LicenseNumber { get; set; } = null!;
        public string LicenseClass { get; set; } = null!;
        public string LicenseClassName { get; set; } = null!;
        public int DriverStatusId { get; set; }
        public string DriverStatusName { get; set; } = null!;
        public string DriverStatusColor { get; set; } = null!;
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; } = null!;
        //public int? AvatarId { get; set; }
        //public string? AvatarUrl { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }


}