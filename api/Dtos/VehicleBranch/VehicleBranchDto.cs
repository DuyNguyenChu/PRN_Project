using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.VehicleBranch
{
    public class VehicleBranchDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public int? CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }

}