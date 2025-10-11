using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.DTParameters;
using api.Extensions;
using api.Models;
using api.ViewModel;

namespace api.Interface.Repository
{
    public interface IUserRepository : IRepositoryBase<User, int>
    {
        Task<DTResult<UserAggregates>> GetPagedAsync(UserDTParameters parameters);
        Task<List<MenuAggregate>> GetMenuAsync(int userId);
        Task<List<PermissionAggregate>> GetPermissionsAsync(int userId);

    }
}
