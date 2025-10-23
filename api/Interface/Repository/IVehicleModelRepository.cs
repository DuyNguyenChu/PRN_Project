using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interface.Repository
{
    public interface IVehicleModelRepository
    {
        Task<VehicleModel?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleModel>> SearchAsync(string name);
        Task<IEnumerable<VehicleModel>> GetAllAsync();
        Task AddAsync(VehicleModel vehicleModel);
        void Update(VehicleModel vehicleModel);
        void Remove(VehicleModel vehicleModel);
        Task SaveChangesAsync();
    }
}