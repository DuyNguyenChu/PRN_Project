using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interface.Repository;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class VehicleBranchRepository : IVehicleBranchRepository
    {
        private readonly PrnprojectContext _context;

        public VehicleBranchRepository(PrnprojectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VehicleBranch>> GetAllAsync()
        {
            return await _context.VehicleBranches
                .Include(vr => vr.Vehicles)
                .ToListAsync();
        }

        public async Task<VehicleBranch?> GetByIdAsync(int id)
        {
            return await _context.VehicleBranches
                .Include(vr => vr.Vehicles)
                .FirstOrDefaultAsync(vr => vr.Id == id);
        }

        public async Task<IEnumerable<VehicleBranch>> SearchAsync(string name)
        {
            return await _context.VehicleBranches
                .Include(vr => vr.Vehicles)
                .Where(vr => vr.Vehicles.Any(v => v.Name.Contains(name)))
                .ToListAsync();
        }

        public async Task AddAsync(VehicleBranch vehicleBranch)
        {
            await _context.VehicleBranches.AddAsync(vehicleBranch);
        }

        public void Update(VehicleBranch vehicleBranch)
        {
            _context.VehicleBranches.Update(vehicleBranch);
        }

        public void Remove(VehicleBranch vehicleBranch)
        {
            _context.VehicleBranches.Remove(vehicleBranch);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}