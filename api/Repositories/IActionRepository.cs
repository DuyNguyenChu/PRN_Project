using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Extensions;
using api.Interface.Repository;
using api.ViewModel;


namespace api.Repositories
{
    public interface IActionRepository : IRepositoryBase<Models.Action, int>
    {
        Task<DTResult<ActionAggregate>> GetPagedAsync(DTParameters parameters);
    }
}
