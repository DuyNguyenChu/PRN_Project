using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Permission
{
    public class PermissionListDto
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public string ActionName { get; set; } = null!;
        public int MenuId { get; set; }
        public string MenuName { get; set; } = null!;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;

    }
}
