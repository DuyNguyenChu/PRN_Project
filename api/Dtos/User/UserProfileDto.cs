using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Extensions;

namespace api.Dtos.User
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public DataItem<int> UserStatus { get; set; }
        public List<DataItem<int>> UserRoles { get; set; }
        //public FileUploadDetailDto? Avatar { get; set; }

    }
}
