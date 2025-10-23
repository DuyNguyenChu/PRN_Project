using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Vehicle
{
    public class VehicleUpdateDto
    {
        public string Name { get; set; } = null!;
        public int VehicleTypeId { get; set; }
        public int VehicleStatusId { get; set; }
        public int VehicleBranchId { get; set; }
        public int VehicleModelId { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? Color { get; set; }
        public int? ManufactureYear { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }

}