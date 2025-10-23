using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.VehicleRegistration
{
    public class VehicleRegistrationDto
    {
        public int Id { get; set; }  // Kế thừa từ EntityBase<int>

        public int VehicleId { get; set; }

        public string RegistrationNumber { get; set; } = null!;

        public DateTimeOffset IssueDate { get; set; }

        public DateTimeOffset ExpiryDate { get; set; }

        public int Status { get; set; }

        public int? CreatedBy { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset? LastModifiedDate { get; set; }

        public int? UpdatedBy { get; set; }

        // Có thể include thông tin xe nếu cần
        public string? VehicleName { get; set; }
        public bool IsDeleted { get; set; }
    }

}