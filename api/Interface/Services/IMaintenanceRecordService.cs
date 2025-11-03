using api.Dtos.MaintenanceRecord;
using api.DTParameters;
using api.Helpers;

namespace api.Interface.Services
{
    public interface IMaintenanceRecordService
    {
        Task<ApiResponse> CreateAsync(CreateMaintenanceRecordDto obj);
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> GetPagedAsync(MaintenanceRecordDTParameters parameters);
        Task<ApiResponse> UpdateAsync(UpdateMaintenanceRecordDto obj);
        Task<ApiResponse> RejectAsync(RejectMaintenanceRecordDto obj);
        Task<ApiResponse> ApproveAsync(int id);
        Task<ApiResponse> GetServiceTypes();
    }
}