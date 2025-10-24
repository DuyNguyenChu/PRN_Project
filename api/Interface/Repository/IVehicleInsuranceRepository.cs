using api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Interfaces.Repository
{
    public interface IVehicleInsuranceRepository
    {
        Task<IEnumerable<VehicleInsurance>> GetAllAsync();
        Task<VehicleInsurance?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleInsurance>> GetByVehicleIdAsync(int vehicleId);
        Task<IEnumerable<VehicleInsurance>> SearchAsync(string q);
        Task AddAsync(VehicleInsurance entity);
        void Update(VehicleInsurance entity);
        void Remove(VehicleInsurance entity);
        Task SaveChangesAsync();
    }
}