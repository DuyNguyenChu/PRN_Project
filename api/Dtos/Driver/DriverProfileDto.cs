using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Driver
{
    public class DriverProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int? ExperienceYears { get; set; }
        public decimal BaseSalary { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string LicenseClass { get; set; } = string.Empty;
        public DateTimeOffset LicenseExpiryDate { get; set; }
        public string SocialInsuranceNumber { get; set; } = string.Empty;
        //public int? BankId { get; set; }
        //public string? BankBranch { get; set; }
        //public string? BankNumber { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public int DriverStatusId { get; set; }
        public string DriverStatusName { get; set; } = string.Empty;
        public string DriverStatusColor { get; set; } = string.Empty;
        public int UserId { get; set; }

    }

}
