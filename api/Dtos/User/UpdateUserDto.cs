using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.User
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public int? UpdatedBy { get; set; }
        public int UserStatusId { get; set; }
        public List<int> Roles { get; set; } = new List<int>();

    }
}