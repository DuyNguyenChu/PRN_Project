using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.Permission;

namespace api.Interface.Services
{
    public interface IPermissionService : IServiceBase<int, CreatePermissionDto, UpdatePermissionDto, api.Extensions.DTParameters>
    {
    }
}
