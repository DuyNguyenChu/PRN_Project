using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Role
{
    public class RoleListDto : RoleDetailDto
    {
        public int TotalUser { get; set; } = 0;
        public List<RolePermissonDto> Permissons { get; set; } = new List<RolePermissonDto>();

    }
}
