using api.Interfaces.Repository;
using api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Repository
{
    public class VehicleAssignmentRepository : IVehicleAssignmentRepository
    {
        private readonly PrnprojectContext _context;

        public VehicleAssignmentRepository(PrnprojectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VehicleAssignment>> GetAllAsync()
        {
            return await _context.VehicleAssignments.Where(v => !v.IsDeleted).ToListAsync();
        }

        public async Task<VehicleAssignment?> GetByIdAsync(int id)
        {
            return await _context.VehicleAssignments.FindAsync(id);
        }

        public async Task<IEnumerable<VehicleAssignment>> GetByVehicleIdAsync(int vehicleId)
        {
            return await _context.VehicleAssignments
                .Where(x => x.VehicleId == vehicleId && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<VehicleAssignment>> GetByDriverIdAsync(int driverId)
        {
            return await _context.VehicleAssignments
                .Where(x => x.DriverId == driverId && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<VehicleAssignment>> SearchAsync(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return Enumerable.Empty<VehicleAssignment>();

            var list = await _context.VehicleAssignments
                .Where(v => !v.IsDeleted)
                .ToListAsync();

            if (DateTimeOffset.TryParse(q, out var dt))
                return list.Where(x => x.AssignmentDate.Date == dt.Date);

            return list.Where(x =>
                (!string.IsNullOrEmpty(x.Notes) && x.Notes.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                x.AssignmentDate.ToString("yyyy-MM-dd").Contains(q)
            );
        }

        public async Task AddAsync(VehicleAssignment entity)
        {
            await _context.VehicleAssignments.AddAsync(entity);
        }

        public void Update(VehicleAssignment entity)
        {
            _context.VehicleAssignments.Update(entity);
        }

        public void Remove(VehicleAssignment entity)
        {
            _context.VehicleAssignments.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}