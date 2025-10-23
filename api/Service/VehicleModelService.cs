using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleModel;
using api.Interface.Repository;
using api.Interface.Services;
using api.Mappers;

namespace api.Service
{
    public class VehicleModelService : IVehicleModelService
    {
        private readonly IVehicleModelRepository _vehicleModelRepository;
        public VehicleModelService(IVehicleModelRepository vehicleModelRepository)
        {
            _vehicleModelRepository = vehicleModelRepository;
        }

        public async Task<IEnumerable<VehicleModelDto>> GetAllAsync()
        {
            var vehicles = await _vehicleModelRepository.GetAllAsync();
            return vehicles.Select(v => v.ToEntityDto());
        }
        public async Task<IEnumerable<VehicleModelDto>> SearchAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên xe không được để trống.");

            var vehicles = await _vehicleModelRepository.SearchAsync(name);

            // Map từng vehicle sang VehicleDto
            return vehicles.Select(v => v.ToEntityDto());
        }

        public async Task<VehicleModelDto?> GetByIdAsync(int id)
        {
            var vehicle = await _vehicleModelRepository.GetByIdAsync(id);
            return vehicle?.ToEntityDto();
        }

        public async Task<VehicleModelDto> CreateAsync(VehicleModelCreateDto dto)
        {
            var vehicle = dto.ToCreateEntity();
            await _vehicleModelRepository.AddAsync(vehicle);
            await _vehicleModelRepository.SaveChangesAsync();
            return vehicle.ToEntityDto();
        }

        public async Task<bool> UpdateAsync(int id, VehicleModelUpdateDto dto)
        {
            var existing = await _vehicleModelRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing = dto.ToUpdateEntity(existing);
            _vehicleModelRepository.Update(existing);
            await _vehicleModelRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _vehicleModelRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.IsDeleted = true; // xoá mềm
            _vehicleModelRepository.Update(existing);
            await _vehicleModelRepository.SaveChangesAsync();
            return true;
        }

    }
}