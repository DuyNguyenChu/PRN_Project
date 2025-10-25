using api.Dtos.VehicleInsurance;
using api.Interfaces;
using api.Interfaces.Repository;
using api.Mappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Services
{
    public class VehicleInsuranceService : IVehicleInsuranceService
    {
        private readonly IVehicleInsuranceRepository _repo;

        public VehicleInsuranceService(IVehicleInsuranceRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<VehicleInsuranceDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => x.ToDto())!;
        }

        public async Task<VehicleInsuranceDto?> GetByIdAsync(int id)
        {
            var m = await _repo.GetByIdAsync(id);
            return m?.ToDto();
        }

        public async Task<IEnumerable<VehicleInsuranceDto>> GetByVehicleIdAsync(int vehicleId)
        {
            var list = await _repo.GetByVehicleIdAsync(vehicleId);
            return list.Select(x => x.ToDto())!;
        }

        public async Task<VehicleInsuranceDto> CreateAsync(VehicleInsuranceCreateDto dto)
        {
            var model = dto.ToModel();
            await _repo.AddAsync(model);
            await _repo.SaveChangesAsync();
            return model.ToDto()!;
        }

        public async Task<bool> UpdateAsync(int id, VehicleInsuranceUpdateDto dto)
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

        public async Task<IEnumerable<VehicleInsuranceDto>> SearchAsync(string q)
        {
            var list = await _repo.SearchAsync(q);
            return list.Select(x => x.ToDto())!;
        }
    }
}