using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Extensions;
using api.ViewModel;

namespace api.Dtos.Auth
{
    public class LoginResponseDto<T> where T : LoginResponseClientUserInfo
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public T UserInfo { get; set; } = default!;
    }

    public class LoginResponseAdminUserInfo : LoginResponseClientUserInfo
    {
        public List<DataItem<int>> Roles { get; set; } = new List<DataItem<int>>();
        public string? OfficeName { get; set; }
        public List<PermissionAggregate> Permissions { get; set; } = new List<PermissionAggregate>();
        public List<MenuAggregate> Menus { get; set; } = new List<MenuAggregate>();

    }

    public class LoginResponseClientUserInfo
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string Email { get; set; } = null!;
        public int? DriverId { get; set; }
    }

    public class LoginResponsePermission()
    {
        public int MenuId { get; set; }
        public List<int> ActionIds { get; set; } = new List<int>();
    }

}
