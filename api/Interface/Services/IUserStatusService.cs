using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Dtos.UserStatus;
using api.Extensions;

namespace api.Interface.Services
{
    public interface IUserStatusService : IServiceBase<int, CreateUserStatusDto, UpdateUserStatusDto, api.Extensions.DTParameters>
    {
    }
}
