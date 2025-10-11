using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.Menu;
using api.Extensions;
using api.Helpers;

namespace api.Interface.Services
{
    public interface IMenuService : IServiceBase<int, CreateMenuDto, UpdateMenuDto, api.Extensions.DTParameters>
    {
        Task<ApiResponse> GetMenuPermissionAsync();
        Task<ApiResponse> GetAllMenuTypeAsync();

    }
}
