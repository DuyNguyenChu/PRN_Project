using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.Auth
{
    public class ResendActivationMailDto
    {
        public required string Email { get; set; }
        public required string VerificationCode { get; set; }

    }
}
