using api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Interfaces.Repository
{
    public interface IVehicleAssignmentRepository
    {
        Task<IEnumerable<VehicleAssignment>> GetAllAsync();
        Task<VehicleAssignment?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleAssignment>> GetByVehicleIdAsync(int vehicleId);
        Task<IEnumerable<VehicleAssignment>> GetByDriverIdAsync(int driverId);
        Task AddAsync(VehicleAssignment entity);
        void Update(VehicleAssignment entity);
        void Remove(VehicleAssignment entity);
        Task SaveChangesAsync();
        Task<IEnumerable<VehicleAssignment>> SearchAsync(string q);
    }
}