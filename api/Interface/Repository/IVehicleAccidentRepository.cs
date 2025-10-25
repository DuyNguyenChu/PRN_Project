using api.Dtos.VehicleAccident;
using api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Interfaces.Repository
{
    public interface IVehicleAccidentRepository
    {
        Task<VehicleAccident?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleAccident>> SearchAsync(string name);
        Task<IEnumerable<VehicleAccident>> GetAllAsync();
        Task AddAsync(VehicleAccident vehicle);
        void Update(VehicleAccident vehicle);
        void Remove(VehicleAccident vehicle);
        Task SaveChangesAsync();
    }
}