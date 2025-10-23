using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interface.Repository
{
    public interface IVehicleStatusRepository
    {
        Task<VehicleStatus?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleStatus>> SearchAsync(string name);
        Task<IEnumerable<VehicleStatus>> GetAllAsync();
        Task AddAsync(VehicleStatus vehicleStatus);
        void Update(VehicleStatus vehicleStatus);
        void Remove(VehicleStatus vehicleStatus);
        Task SaveChangesAsync();
    }
}