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
    public interface IDriverStatusRepository: IRepositoryBase<DriverStatus,int>
    {
        Task<DTResult<DriverStatusAggregate>> GetPagedAsync(api.Extensions.DTParameters parameters);
    }
}
