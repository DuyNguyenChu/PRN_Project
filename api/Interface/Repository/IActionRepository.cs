using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Extensions;
using api.ViewModel;


namespace api.Interface.Repository
{
    public interface IActionRepository : IRepositoryBase<Models.Action, int>
    {
        Task<DTResult<ActionAggregate>> GetPagedAsync(api.Extensions.DTParameters parameters);
    }
}
