using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interface.Repository;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class VehicleStatusRepository : IVehicleStatusRepository
    {
        private readonly PrnprojectContext _context;

        public VehicleStatusRepository(PrnprojectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VehicleStatus>> GetAllAsync()
        {
            return await _context.VehicleStatuses.ToListAsync();
        }
        public async Task<VehicleStatus?> GetByIdAsync(int id)
        {
            return await _context.VehicleStatuses.FindAsync(id);
        }

        public async Task<IEnumerable<VehicleStatus>> SearchAsync(string name)
        {
            return await _context.VehicleStatuses
                .Where(v => !v.IsDeleted && v.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        public async Task AddAsync(VehicleStatus vehicleStatus)
        {
            await _context.VehicleStatuses.AddAsync(vehicleStatus);
            await _context.SaveChangesAsync();
        }

        public void Update(VehicleStatus vehicleStatus)
        {
            _context.VehicleStatuses.Update(vehicleStatus);
        }

        public void Remove(VehicleStatus vehicleStatus)
        {
            _context.VehicleStatuses.Remove(vehicleStatus);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}