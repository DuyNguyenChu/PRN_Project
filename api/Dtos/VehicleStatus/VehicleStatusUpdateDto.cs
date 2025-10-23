using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.VehicleStatus
{
    public class VehicleStatusUpdateDto
    {
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}