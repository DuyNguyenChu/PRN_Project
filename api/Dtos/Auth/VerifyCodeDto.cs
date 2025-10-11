using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Auth
{
    public class VerifyCodeDto : ForgotPasswordDto
    {
        public string Purpose { get; set; }
        public string Code { get; set; }

    }
}
