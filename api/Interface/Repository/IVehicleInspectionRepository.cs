using api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Interfaces.Repository
{
    public interface IVehicleInspectionRepository
    {
        Task<IEnumerable<VehicleInspection>> GetAllAsync();
        Task<VehicleInspection?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleInspection>> GetByVehicleIdAsync(int vehicleId);
        Task<IEnumerable<VehicleInspection>> GetByInspectorIdAsync(int inspectorId);
        Task<IEnumerable<VehicleInspection>> SearchAsync(string q);
        Task AddAsync(VehicleInspection entity);
        void Update(VehicleInspection entity);
        void Remove(VehicleInspection entity);
        Task SaveChangesAsync();
    }
}