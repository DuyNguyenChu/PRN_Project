using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.UserStatus
{
    public class CreateUserStatusDto
    {
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public int CreatedBy { get; set; }

    }
}
