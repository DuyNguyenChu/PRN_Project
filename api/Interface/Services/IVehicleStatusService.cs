using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleStatus;

namespace api.Interface.Services
{
    public interface IVehicleStatusService
    {
        Task<IEnumerable<VehicleStatusDto>> GetAllAsync();
        Task<IEnumerable<VehicleStatusDto>> SearchAsync(string name);
        Task<VehicleStatusDto?> GetByIdAsync(int id);
        Task<VehicleStatusDto> CreateAsync(VehicleStatusCreateDto dto);
        Task<bool> UpdateAsync(int id, VehicleStatusUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}