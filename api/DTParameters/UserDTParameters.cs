using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.DTParameters
{
    public class UserDTParameters : api.Extensions.DTParameters
    {
        public string? Username { get; set; }
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public List<int> UserStatusIds { get; set; } = new List<int>();
        public List<int> RoleIds { get; set; } = new List<int>();
        //public List<int> GenderIds { get; set; } = new List<int>();
        public int? CurrentUserId { get; set; }
        public bool IsGetAll { get; set; } = true;

    }
}
