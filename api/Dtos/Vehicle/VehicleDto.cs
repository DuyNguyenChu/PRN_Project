using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Vehicle
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int VehicleTypeId { get; set; }
        public int VehicleStatusId { get; set; }
        public int VehicleBranchId { get; set; }
        public int VehicleModelId { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? Color { get; set; }
        public int? ManufactureYear { get; set; }
        public string Description { get; set; } = null!;
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        required public string VehicleTypeName { get; set; } 
        required public string VehicleStatusName { get; set; }
        required public string VehicleBranchName { get; set; }
        required public string VehicleModelName { get; set; }
    }

}