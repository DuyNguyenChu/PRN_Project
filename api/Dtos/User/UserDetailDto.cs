using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Extensions;

namespace api.Dtos.User
{
    public class UserDetailDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public DataItem<int> UserStatus { get; set; } = new DataItem<int>();
        //public virtual FileUploadDetailDto? Avatar { get; set; }
        public List<DataItem<int>>? Roles { get; set; } = new List<DataItem<int>>();
        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset? DateOfBirth { get; set; }

    }
}
