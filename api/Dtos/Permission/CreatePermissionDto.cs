using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Permission
{
    public class CreatePermissionDto
    {
        public int RoleId { get; set; } = 0;
        public int MenuId { get; set; } = 0;
        public int ActionId { get; set; } = 0;
        public int? CreatedBy { get; set; }

    }
}
