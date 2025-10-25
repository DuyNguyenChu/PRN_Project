using api.Dtos.VehicleAssignment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Interfaces
{
    public interface IVehicleAssignmentService
    {
        Task<IEnumerable<VehicleAssignmentDto>> GetAllAsync();
        Task<VehicleAssignmentDto?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleAssignmentDto>> GetByVehicleIdAsync(int vehicleId);
        Task<IEnumerable<VehicleAssignmentDto>> GetByDriverIdAsync(int driverId);
        Task<VehicleAssignmentDto> CreateAsync(VehicleAssignmentCreateDto dto);
        Task<bool> UpdateAsync(int id, VehicleAssignmentUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<VehicleAssignmentDto>> SearchAsync(string q);
    }
}