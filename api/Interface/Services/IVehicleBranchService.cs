using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.VehicleBranch;

namespace api.Interface.Services
{
    public interface IVehicleBranchService
    {
        Task<VehicleBranchDto?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleBranchDto>> SearchAsync(string name);
        Task<IEnumerable<VehicleBranchDto>> GetAllAsync();
        Task<VehicleBranchDto> CreateAsync(VehicleBranchCreateDto dto);
        Task<bool> UpdateAsync(int id, VehicleBranchUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}