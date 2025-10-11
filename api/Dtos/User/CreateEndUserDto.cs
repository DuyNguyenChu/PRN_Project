using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.User
{
    public class CreateEndUserDto
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public int? CreatedBy { get; set; }
        //update
        public string DateOfBirth { get; set; }
        public int Gender { get; set; }

    }
}
