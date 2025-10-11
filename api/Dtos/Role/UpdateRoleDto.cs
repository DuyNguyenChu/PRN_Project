using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Role
{
    public class UpdateRoleDto : CreateRoleDto
    {
        public int Id { get; set; }
        public int? UpdatedBy { get; set; }

    }
}
