using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interface.Repository;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class VehicleRegistrationRepository : IVehicleRegistrationRepository
    {
        private readonly PrnprojectContext _context;

        public VehicleRegistrationRepository(PrnprojectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VehicleRegistration>> GetAllAsync()
        {
            return await _context.VehicleRegistrations
                .Include(vr => vr.Vehicle)
                .ToListAsync();
        }

        public async Task<VehicleRegistration?> GetByIdAsync(int id)
        {
            return await _context.VehicleRegistrations
                .Include(vr => vr.Vehicle)
                .FirstOrDefaultAsync(vr => vr.Id == id);
        }

        public async Task<IEnumerable<VehicleRegistration>> SearchVehiclesAsync(string name)
        {
            return await _context.VehicleRegistrations
                .Include(vr => vr.Vehicle)
                .Where(vr => vr.Vehicle.Name.Contains(name))
                .ToListAsync();
        }

        public async Task AddAsync(VehicleRegistration vehicleRegistration)
        {
            await _context.VehicleRegistrations.AddAsync(vehicleRegistration);
        }

        public void Update(VehicleRegistration vehicleRegistration)
        {
            _context.VehicleRegistrations.Update(vehicleRegistration);
        }

        public void Remove(VehicleRegistration vehicleRegistration)
        {
            _context.VehicleRegistrations.Remove(vehicleRegistration);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}