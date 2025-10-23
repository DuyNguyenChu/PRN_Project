using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interface.Repository;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class VehicleModelRepository : IVehicleModelRepository
    {
        private readonly PrnprojectContext _context;

        public VehicleModelRepository(PrnprojectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VehicleModel>> GetAllAsync()
        {
            return await _context.VehicleModels.ToListAsync();
        }
        public async Task<VehicleModel?> GetByIdAsync(int id)
        {
            return await _context.VehicleModels.FindAsync(id);
        }

        public async Task<IEnumerable<VehicleModel>> SearchAsync(string name)
        {
            return await _context.VehicleModels
                .Where(v => !v.IsDeleted && v.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        public async Task AddAsync(VehicleModel vehicleModel)
        {
            await _context.VehicleModels.AddAsync(vehicleModel);
            await _context.SaveChangesAsync();
        }

        public void Update(VehicleModel vehicleModel)
        {
            _context.VehicleModels.Update(vehicleModel);
        }

        public void Remove(VehicleModel vehicleModel)
        {
            _context.VehicleModels.Remove(vehicleModel);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}