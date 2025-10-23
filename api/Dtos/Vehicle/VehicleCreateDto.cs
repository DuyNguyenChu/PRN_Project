using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Vehicle
{
    public class VehicleCreateDto
    {
        public string Name { get; set; } = null!;
        public int VehicleTypeId { get; set; }
        public int VehicleStatusId { get; set; }
        public int VehicleBranchId { get; set; }
        public int VehicleModelId { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? IdentificationNumber { get; set; }
        public string? EngineNumber { get; set; }
        public string? Color { get; set; }
        public int? ManufactureYear { get; set; }
        public string Description { get; set; } = string.Empty;
    }

}