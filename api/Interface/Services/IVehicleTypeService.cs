using api.Dtos.VehicleType;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Interfaces
{
    public interface IVehicleTypeService
    {
        Task<IEnumerable<VehicleTypeDto>> GetAllAsync();
        Task<VehicleTypeDto?> GetByIdAsync(int id);
        Task<VehicleTypeDto> CreateAsync(VehicleTypeCreateDto dto);
        Task<bool> UpdateAsync(int id, VehicleTypeUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<VehicleTypeDto>> SearchAsync(string q);
    }
}