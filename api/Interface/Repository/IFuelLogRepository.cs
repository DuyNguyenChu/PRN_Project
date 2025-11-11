using api.Dtos.FuelLog;
using api.DTParameters;
using api.Extensions;
using api.Helpers;
using api.Models;
using api.ViewModel;

namespace api.Interface.Repository
{
    public interface IFuelLogRepository : IRepositoryBase<FuelLog, int>
    {
        Task<PagingData<FuelLogAggregate>> GetFuelLogsAsync(FuelLogSearchQuery query);
        Task<DTResult<FuelLogAggregate>> GetPagedAsync(FuelLogDTParameters parameters);
    }
}