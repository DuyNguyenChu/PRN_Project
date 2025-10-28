using api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Interfaces.Repository
{
    public interface IVehicleTypeRepository
    {
        Task<IEnumerable<VehicleType>> GetAllAsync();
        Task<VehicleType?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleType>> SearchAsync(string q);
        Task AddAsync(VehicleType entity);
        void Update(VehicleType entity);
        void Remove(VehicleType entity);
        Task SaveChangesAsync();
    }
}