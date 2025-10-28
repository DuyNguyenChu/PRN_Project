using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Vehicle;
using api.Interface.Repository;
using api.Interface.Services;
using api.Mappers;

namespace api.Service
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        public VehicleService(IVehicleRepository VehicleRepository)
        {
            _vehicleRepository = VehicleRepository;
        }

        public async Task<IEnumerable<VehicleDto>> GetAllAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return vehicles.Select(v => v.ToEntityDto());
        }
        
        public async Task<IEnumerable<VehicleDto>> SearchAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên xe không được để trống.");

            var vehicles = await _vehicleRepository.SearchAsync(name);

            // Map từng vehicle sang VehicleDto
            return vehicles.Select(v => v.ToEntityDto());
        }

        public async Task<VehicleDto?> GetByIdAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            return vehicle?.ToEntityDto();
        }

        public async Task<VehicleDto> CreateAsync(VehicleCreateDto dto)
        {
            var vehicle = dto.ToCreateEntity();
            await _vehicleRepository.AddAsync(vehicle);
            await _vehicleRepository.SaveChangesAsync();
            return vehicle.ToEntityDto();
        }

        public async Task<bool> UpdateAsync(int id, VehicleUpdateDto dto)
        {
            var existing = await _vehicleRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing = dto.ToUpdateEntity(existing);
            _vehicleRepository.Update(existing);
            await _vehicleRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _vehicleRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.IsDeleted = true; // xoá mềm
            _vehicleRepository.Update(existing);
            await _vehicleRepository.SaveChangesAsync();
            return true;
        }
    }
}