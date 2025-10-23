using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interface.Repository
{
    public interface IVehicleRegistrationRepository
    {
        Task<VehicleRegistration?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleRegistration>> SearchVehiclesAsync(string name);
        Task<IEnumerable<VehicleRegistration>> GetAllAsync();
        Task AddAsync(VehicleRegistration vehicleRegistration);
        void Update(VehicleRegistration vehicleRegistration);
        void Remove(VehicleRegistration vehicleRegistration);
        Task SaveChangesAsync();
    }
}