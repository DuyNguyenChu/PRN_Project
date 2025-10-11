using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Auth
{
    public class ForgotPasswordDto
    {
        public string Email { get; set; }
        public bool IsClientRequest { get; set; } = false;

    }
}
