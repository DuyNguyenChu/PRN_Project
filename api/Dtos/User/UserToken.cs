using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.User
{
    public class UserToken
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AccessTokenJti { get; set; } = Guid.NewGuid().ToString();
        public int? DriverId { get; set; }
        public List<int> RoleIds { get; set; } = new List<int>();
        public string FullName { get; set; } = string.Empty;

    }
}
