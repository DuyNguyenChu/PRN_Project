using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.UserStatus
{
    public class UpdateUserStatusDto : CreateUserStatusDto
    {
        public int Id { get; set; }
        public int? UpdatedBy { get; set; }

    }
}
