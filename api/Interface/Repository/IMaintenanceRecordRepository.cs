using api.Models;
using api.ViewModel;
using api.DTParameters;
using api.Extensions;

namespace api.Interface.Repository
{
    public interface IMaintenanceRecordRepository : IRepositoryBase<MaintenanceRecord, int>
    {
        Task<DTResult<MaintenanceRecordAggregate>> GetPagedAsync(MaintenanceRecordDTParameters parameters);
    }
}