using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleRegistration;
using api.Interface.Repository;
using api.Interface.Services;
using api.Mappers;


namespace api.Service
{
    public class VehicleRegistrationService : IVehicleRegistrationService
    {
        private readonly IVehicleRegistrationRepository _vehicleRegistrationRepository;
        public VehicleRegistrationService(IVehicleRegistrationRepository vehicleRegistrationRepository)
        {
            _vehicleRegistrationRepository = vehicleRegistrationRepository;
        }

        public async Task<IEnumerable<VehicleRegistrationDto>> GetAllAsync()
        {
            var vehicles = await _vehicleRegistrationRepository.GetAllAsync();
            return vehicles.Select(v => v.ToEntityDto());
        }
        public async Task<IEnumerable<VehicleRegistrationDto>> SearchVehiclesAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên xe không được để trống.");

            var vehicles = await _vehicleRegistrationRepository.SearchVehiclesAsync(name);

            // Map từng vehicle sang VehicleDto
            return vehicles.Select(v => v.ToEntityDto());
        }

        public async Task<VehicleRegistrationDto?> GetByIdAsync(int id)
        {
            var vehicle = await _vehicleRegistrationRepository.GetByIdAsync(id);
            return vehicle?.ToEntityDto();
        }

        public async Task<VehicleRegistrationDto> CreateAsync(VehicleRegistrationCreateDto dto)
        {
            var vehicle = dto.ToCreateEntity();
            await _vehicleRegistrationRepository.AddAsync(vehicle);
            await _vehicleRegistrationRepository.SaveChangesAsync();
            return vehicle.ToEntityDto();
        }
        public async Task<bool> UpdateAsync(int id, VehicleRegistrationUpdateDto dto)
        {
            var existing = await _vehicleRegistrationRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing = dto.ToUpdateEntity(existing);
            _vehicleRegistrationRepository.Update(existing);
            await _vehicleRegistrationRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _vehicleRegistrationRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.IsDeleted = true; // xoá mềm
            _vehicleRegistrationRepository.Update(existing);
            await _vehicleRegistrationRepository.SaveChangesAsync();
            return true;
        }
    }
}