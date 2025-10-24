using api.Dtos.VehicleAssignment;
using api.Interfaces;
using api.Interfaces.Repository;
using api.Mappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Services
{
    public class VehicleAssignmentService : IVehicleAssignmentService
    {
        private readonly IVehicleAssignmentRepository _repo;

        public VehicleAssignmentService(IVehicleAssignmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<VehicleAssignmentDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => x.ToDto())!;
        }

        public async Task<VehicleAssignmentDto?> GetByIdAsync(int id)
        {
            var m = await _repo.GetByIdAsync(id);
            return m?.ToDto();
        }

        public async Task<IEnumerable<VehicleAssignmentDto>> GetByVehicleIdAsync(int vehicleId)
        {
            var list = await _repo.GetByVehicleIdAsync(vehicleId);
            return list.Select(x => x.ToDto())!;
        }

        public async Task<IEnumerable<VehicleAssignmentDto>> GetByDriverIdAsync(int driverId)
        {
            var list = await _repo.GetByDriverIdAsync(driverId);
            return list.Select(x => x.ToDto())!;
        }

        public async Task<VehicleAssignmentDto> CreateAsync(VehicleAssignmentCreateDto dto)
        {
            var model = dto.ToModel();
            await _repo.AddAsync(model);
            await _repo.SaveChangesAsync();
            return model.ToDto()!;
        }

        public async Task<bool> UpdateAsync(int id, VehicleAssignmentUpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;
            existing.ApplyUpdate(dto);
            _repo.Update(existing);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;
            existing.IsDeleted = true;
            _repo.Update(existing);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<VehicleAssignmentDto>> SearchAsync(string q)
        {
            var list = await _repo.SearchAsync(q);
            return list.Select(x => x.ToDto())!;
        }
    }
}