using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.Permission;
using api.Dtos.Role;
using api.Helpers;

namespace api.Interface.Services
{
    public interface IRoleService : IServiceBase<int, CreateRoleDto, UpdateRoleDto, api.Extensions.DTParameters>
    {
        Task<ApiResponse> GetPermissionAsync(int roleId);
        Task<ApiResponse> UpdatePermissionAsync(int roleId, List<UpdatePermissionDto> objs);
        Task<ApiResponse> GetByAdminRegister();
        Task<ApiResponse> GetListRoleForUserManagementAsync();

    }
}
