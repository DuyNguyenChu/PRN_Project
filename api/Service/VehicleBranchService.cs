using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleBranch;
using api.Interface.Repository;
using api.Interface.Services;
using api.Mappers;

namespace api.Service
{
    public class VehicleBranchService : IVehicleBranchService
    {
        private readonly IVehicleBranchRepository _vehicleBranchRepository;
        public VehicleBranchService(IVehicleBranchRepository vehicleBranchRepository)
        {
            _vehicleBranchRepository = vehicleBranchRepository;
        }

        public async Task<IEnumerable<VehicleBranchDto>> GetAllAsync()
        {
            var vehicles = await _vehicleBranchRepository.GetAllAsync();
            return vehicles.Select(v => v.ToEntityDto());
        }
        public async Task<IEnumerable<VehicleBranchDto>> SearchAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên xe không được để trống.");

            var vehicles = await _vehicleBranchRepository.SearchAsync(name);

            // Map từng vehicle sang VehicleDto
            return vehicles.Select(v => v.ToEntityDto());
        }

        public async Task<VehicleBranchDto?> GetByIdAsync(int id)
        {
            var vehicle = await _vehicleBranchRepository.GetByIdAsync(id);
            return vehicle?.ToEntityDto();
        }

        public async Task<VehicleBranchDto> CreateAsync(VehicleBranchCreateDto dto)
        {
            var entity = dto.ToCreateEntity();
            await _vehicleBranchRepository.AddAsync(entity);
            await _vehicleBranchRepository.SaveChangesAsync();
            return entity.ToEntityDto();
        }
        public async Task<bool> UpdateAsync(int id, VehicleBranchUpdateDto dto)
        {
            var existing = await _vehicleBranchRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing = dto.ToUpdateEntity(existing);
            _vehicleBranchRepository.Update(existing);
            await _vehicleBranchRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _vehicleBranchRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.IsDeleted = true;
            _vehicleBranchRepository.Update(existing);
            await _vehicleBranchRepository.SaveChangesAsync();
            return true;
        }
    }
}