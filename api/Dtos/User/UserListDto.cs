using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Dtos.User
{
    public class UserListDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public int UserStatusId { get; set; }
        public string UserStatusName { get; set; } = null!;
        public int? AvatarId { get; set; }
        public string? AvatarKey { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset? DateOfBirth { get; set; }

    }
}
