using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Role
{
    public class CreateRoleDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
        public bool IsCheckAll { get; set; }
        public List<RoleMenuActionDto> Permissions { get; set; } = new List<RoleMenuActionDto>();

    }
}
