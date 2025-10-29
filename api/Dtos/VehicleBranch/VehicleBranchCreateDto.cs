using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.VehicleBranch
{
    public class VehicleBranchCreateDto
    {
        public string Name { get; set; } = null!;
        public DateTimeOffset? LastModifiedDate { get; set; }
        public string? Description { get; set; }
    }

}