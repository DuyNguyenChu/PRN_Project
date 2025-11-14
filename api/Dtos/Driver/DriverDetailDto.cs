using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Driver
{
    public class DriverDetailDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public int? ExperienceYears { get; set; }
        public decimal BaseSalary { get; set; }
        public string LicenseNumber { get; set; } = null!;
        public string LicenseClass { get; set; } = null!;
        public DateTimeOffset LicenseExpiryDate { get; set; }
        public string SocialInsuranceNumber { get; set; } = null!;
        //public string? BankBranch { get; set; }
        //public string? BankNumber { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public int DriverStatusId { get; set; }
        public string DriverStatusName { get; set; } = null!;
        public string DriverStatusColor { get; set; } = null!;
        //public int? BankId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int? Gender { get; set; }
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; } = null!;
        public DateTimeOffset? DateOfBirth { get; set; }
        //public string? IdentityNumber { get; set; }
        //public int AccessFailedCount { get; set; }
        //public bool LockEnabled { get; set; }
        //public DateTimeOffset? LockEndDate { get; set; }
        //public int? AvatarId { get; set; }
        //public string? AvatarUrl { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }

}
