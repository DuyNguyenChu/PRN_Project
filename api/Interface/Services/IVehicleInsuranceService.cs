using api.Dtos.VehicleInsurance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Interfaces
{
    public interface IVehicleInsuranceService
    {
        Task<IEnumerable<VehicleInsuranceDto>> GetAllAsync();
        Task<VehicleInsuranceDto?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleInsuranceDto>> GetByVehicleIdAsync(int vehicleId);
        Task<VehicleInsuranceDto> CreateAsync(VehicleInsuranceCreateDto dto);
        Task<bool> UpdateAsync(int id, VehicleInsuranceUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<VehicleInsuranceDto>> SearchAsync(string q);
    }
}