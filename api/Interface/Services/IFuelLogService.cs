using api.Dtos.FuelLog;
using api.DTParameters;
using api.Helpers;

namespace api.Interface.Services
{
    public interface IFuelLogService
    {
        Task<ApiResponse> CreateAsync(CreateFuelLogDto obj);
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> UpdateAsync(UpdateFuelLogDto obj);
        Task<ApiResponse> SoftDeleteAsync(int id);
        Task<ApiResponse> GetPagedAsync(FuelLogDTParameters parameters);
        Task<ApiResponse> RejectAsync(RejectFuelLogDto obj);
        Task<ApiResponse> ApproveAsync(int id);
        Task<ApiResponse> GetFuelTypes();
        Task<ApiResponse> GetStatus();
    }
}