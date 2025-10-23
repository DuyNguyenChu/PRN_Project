using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleStatus;
using api.Interface.Repository;
using api.Interface.Services;
using api.Mappers;

namespace api.Service
{
    public class VehicleStatusService : IVehicleStatusService
    {
        private readonly IVehicleStatusRepository _vehicleStatusRepository;
        public VehicleStatusService(IVehicleStatusRepository vehicleStatusRepository)
        {
            _vehicleStatusRepository = vehicleStatusRepository;
        }

        public async Task<IEnumerable<VehicleStatusDto>> GetAllAsync()
        {
            var vehicles = await _vehicleStatusRepository.GetAllAsync();
            return vehicles.Select(v => v.ToEntityDto());
        }
        public async Task<IEnumerable<VehicleStatusDto>> SearchAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên xe không được để trống.");

            var vehicles = await _vehicleStatusRepository.SearchAsync(name);

            // Map từng vehicle sang VehicleDto
            return vehicles.Select(v => v.ToEntityDto());
        }

        public async Task<VehicleStatusDto?> GetByIdAsync(int id)
        {
            var vehicle = await _vehicleStatusRepository.GetByIdAsync(id);
            return vehicle?.ToEntityDto();
        }

        public async Task<VehicleStatusDto> CreateAsync(VehicleStatusCreateDto dto)
        {
            var vehicle = dto.ToCreateEntity();
            await _vehicleStatusRepository.AddAsync(vehicle);
            await _vehicleStatusRepository.SaveChangesAsync();
            return vehicle.ToEntityDto();
        }

        public async Task<bool> UpdateAsync(int id, VehicleStatusUpdateDto dto)
        {
            var existing = await _vehicleStatusRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing = dto.ToUpdateEntity(existing);
            _vehicleStatusRepository.Update(existing);
            await _vehicleStatusRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _vehicleStatusRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.IsDeleted = true; // xoá mềm
            _vehicleStatusRepository.Update(existing);
            await _vehicleStatusRepository.SaveChangesAsync();
            return true;
        }
    }
}