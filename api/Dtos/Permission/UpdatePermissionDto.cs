using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Permission
{
    public class UpdatePermissionDto : CreatePermissionDto
    {
        public int? UpdatedBy { get; set; }
    }
}
