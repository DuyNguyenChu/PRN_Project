using api.Dtos.VehicleAccident;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Interfaces;

public interface IVehicleAccidentService
{
        Task<IEnumerable<VehicleAccidentDto>> GetAllAsync();
        Task<IEnumerable<VehicleAccidentDto>> SearchAsync(string name);
        Task<VehicleAccidentDto?> GetByIdAsync(int id);
        Task<VehicleAccidentDto> CreateAsync(VehicleAccidentCreateDto dto);
        Task<bool> UpdateAsync(int id, VehicleAccidentUpdateDto dto);
        Task<bool> DeleteAsync(int id);
}