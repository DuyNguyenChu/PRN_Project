using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleRegistration;

namespace api.Interface.Services
{
    public interface IVehicleRegistrationService
    {
        Task<VehicleRegistrationDto?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleRegistrationDto>> SearchVehiclesAsync(string name);
        Task<IEnumerable<VehicleRegistrationDto>> GetAllAsync();
        Task<VehicleRegistrationDto> CreateAsync(VehicleRegistrationCreateDto dto);
        Task<bool> UpdateAsync(int id, VehicleRegistrationUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}