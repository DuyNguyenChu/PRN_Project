using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Driver
{
    public class UpdateDriverProfileDto
    {
        // Thuộc tính User
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        //public int AvatarId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public int GenderId { get; set; }
        //public string AddressDetail { get; set; } = null!;
        public DateTimeOffset? DateOfBirth { get; set; }

        // Thuộc tính Driver
        public int? ExperienceYears { get; set; }
        public string LicenseNumber { get; set; } = null!;
        public string LicenseClass { get; set; } = null!;
        public DateTimeOffset LicenseExpiryDate { get; set; }
        public string SocialInsuranceNumber { get; set; } = null!;
        //public int? BankId { get; set; }
        //public string? BankBranch { get; set; }
        //public string? BankNumber { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        //public string IdentityNumber { get; set; } = null!;
    }

}
