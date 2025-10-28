using api.Dtos.VehicleInspection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Interfaces
{
    public interface IVehicleInspectionService
    {
        Task<IEnumerable<VehicleInspectionDto>> GetAllAsync();
        Task<VehicleInspectionDto?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleInspectionDto>> GetByVehicleIdAsync(int vehicleId);
        Task<IEnumerable<VehicleInspectionDto>> GetByInspectorIdAsync(int inspectorId);
        Task<VehicleInspectionDto> CreateAsync(VehicleInspectionCreateDto dto);
        Task<bool> UpdateAsync(int id, VehicleInspectionUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<VehicleInspectionDto>> SearchAsync(string q);
    }
}