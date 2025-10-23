using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleModel;

namespace api.Interface.Services
{
    public interface IVehicleModelService
    {
        Task<IEnumerable<VehicleModelDto>> GetAllAsync();
        Task<IEnumerable<VehicleModelDto>> SearchAsync(string name);
        Task<VehicleModelDto?> GetByIdAsync(int id);
        Task<VehicleModelDto> CreateAsync(VehicleModelCreateDto dto);
        Task<bool> UpdateAsync(int id, VehicleModelUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}