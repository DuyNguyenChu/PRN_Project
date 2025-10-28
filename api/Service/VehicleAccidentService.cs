// ...existing code...
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleAccident;
using api.Interfaces.Repository;
using api.Mappers;
using api.Models;
using api.Interfaces;
namespace api.Services
{
    public class VehicleAccidentService : IVehicleAccidentService
    {
        private readonly IVehicleAccidentRepository _vehicleAccidentRepo;

        public VehicleAccidentService(IVehicleAccidentRepository vehicleAccidentRepo)
        {
            _vehicleAccidentRepo = vehicleAccidentRepo;
        }

        public async Task<IEnumerable<VehicleAccidentDto>> GetAllAsync()
        {
            var accidents = await _vehicleAccidentRepo.GetAllAsync();
            return accidents.Select(va => va.ToVehicleAccidentDto()).ToList();
        }

        public async Task<VehicleAccidentDto?> GetByIdAsync(int id)
        {
            var accident = await _vehicleAccidentRepo.GetByIdAsync(id);
            return accident?.ToVehicleAccidentDto();
        }

        public async Task<VehicleAccidentDto> CreateAsync(VehicleAccidentCreateDto createDto)
        {
            var model = createDto.ToVehicleAccidentFromCreateDTO();
            await _vehicleAccidentRepo.AddAsync(model);
            await _vehicleAccidentRepo.SaveChangesAsync();
            return model.ToVehicleAccidentDto();
        }

        public async Task<bool> UpdateAsync(int id, VehicleAccidentUpdateDto updateDto)
        {
            var existing = await _vehicleAccidentRepo.GetByIdAsync(id);
            if (existing == null) return false;

            existing = updateDto.ToVehicleAccidentFromUpdateDTO(existing);
            _vehicleAccidentRepo.Update(existing);
            await _vehicleAccidentRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _vehicleAccidentRepo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.IsDeleted = true; // xoá mềm
            _vehicleAccidentRepo.Update(existing);
            await _vehicleAccidentRepo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<VehicleAccidentDto>> SearchAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên xe không được để trống.");

            var vehicles = await _vehicleAccidentRepo.SearchAsync(name);

            // Map từng vehicle sang VehicleDto
            return vehicles.Select(v => v.ToVehicleAccidentDto());
        }

        
    }
}
