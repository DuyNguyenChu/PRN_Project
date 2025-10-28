using api.Interfaces.Repository;
using api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Repository
{
    public class VehicleInspectionRepository : IVehicleInspectionRepository
    {
        private readonly PrnprojectContext _context;

        public VehicleInspectionRepository(PrnprojectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VehicleInspection>> GetAllAsync()
        {
            return await _context.VehicleInspections.Where(v => !v.IsDeleted).ToListAsync();
        }

        public async Task<VehicleInspection?> GetByIdAsync(int id)
        {
            return await _context.VehicleInspections.FindAsync(id);
        }

        public async Task<IEnumerable<VehicleInspection>> GetByVehicleIdAsync(int vehicleId)
        {
            return await _context.VehicleInspections
                .Where(x => x.VehicleId == vehicleId && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<VehicleInspection>> GetByInspectorIdAsync(int inspectorId)
        {
            return await _context.VehicleInspections
                .Where(x => x.InspectorId == inspectorId && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<VehicleInspection>> SearchAsync(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return Enumerable.Empty<VehicleInspection>();

            var list = await _context.VehicleInspections.Where(v => !v.IsDeleted).ToListAsync();

            if (DateTimeOffset.TryParse(q, out var dt))
                return list.Where(x => x.InspectionDate.Date == dt.Date);

            return list.Where(x =>
                (!string.IsNullOrEmpty(x.Notes) && x.Notes.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(x.Result) && x.Result.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                x.InspectionDate.ToString("yyyy-MM-dd").Contains(q)
            );
        }

        public async Task AddAsync(VehicleInspection entity)
        {
            await _context.VehicleInspections.AddAsync(entity);
        }

        public void Update(VehicleInspection entity)
        {
            _context.VehicleInspections.Update(entity);
        }

        public void Remove(VehicleInspection entity)
        {
            _context.VehicleInspections.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}