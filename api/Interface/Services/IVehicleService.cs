using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Vehicle;
using api.Helpers;

namespace api.Interface.Services
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> GetAllAsync();
        Task<IEnumerable<VehicleDto>> SearchAsync(string name);
        Task<VehicleDto?> GetByIdAsync(int id);
        Task<VehicleDto> CreateAsync(VehicleCreateDto dto);
        Task<bool> UpdateAsync(int id, VehicleUpdateDto dto);
        Task<bool> DeleteAsync(int id);

        Task<ApiResponse> GetVehicleAvailableAsync();
    }
}