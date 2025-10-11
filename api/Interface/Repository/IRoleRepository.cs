using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Extensions;
using api.Models;
using api.ViewModel;

namespace api.Interface.Repository
{
    public interface IRoleRepository : IRepositoryBase<Role, int>
    {
        Task<DTResult<RoleAggregate>> GetPagedAsync(api.Extensions.DTParameters parameters);
    }

}
