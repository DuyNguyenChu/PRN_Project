using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.VehicleRegistration
{
    public class VehicleRegistrationUpdateDto
    {
        public int Id { get; set; }

        public string RegistrationNumber { get; set; } = null!;

        public DateTimeOffset IssueDate { get; set; }

        public DateTimeOffset ExpiryDate { get; set; }

        public int Status { get; set; }
    }

}