using api.Interfaces.Repository;
using api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Repository
{
    public class VehicleTypeRepository : IVehicleTypeRepository
    {
        private readonly PrnprojectContext _context;

        public VehicleTypeRepository(PrnprojectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VehicleType>> GetAllAsync()
        {
            return await _context.VehicleTypes.Where(v => !v.IsDeleted).ToListAsync();
        }

        public async Task<VehicleType?> GetByIdAsync(int id)
        {
            return await _context.VehicleTypes.FindAsync(id);
        }

        public async Task<IEnumerable<VehicleType>> SearchAsync(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return Enumerable.Empty<VehicleType>();

            var list = await _context.VehicleTypes.Where(v => !v.IsDeleted).ToListAsync();
            return list.Where(x =>
                (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(x.Color) && x.Color.Contains(q, StringComparison.OrdinalIgnoreCase))
            );
        }

        public async Task AddAsync(VehicleType entity)
        {
            await _context.VehicleTypes.AddAsync(entity);
        }

        public void Update(VehicleType entity)
        {
            _context.VehicleTypes.Update(entity);
        }

        public void Remove(VehicleType entity)
        {
            _context.VehicleTypes.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}