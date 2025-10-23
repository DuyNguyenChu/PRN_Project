using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interface.Repository
{
    public interface IVehicleBranchRepository
    {
        Task<IEnumerable<VehicleBranch>> GetAllAsync();
        Task<VehicleBranch?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleBranch>> SearchAsync(string name);
        Task AddAsync(VehicleBranch vehicleBranch);
        void Update(VehicleBranch vehicleBranch);
        void Remove(VehicleBranch vehicleBranch);
        Task SaveChangesAsync();
    }
}