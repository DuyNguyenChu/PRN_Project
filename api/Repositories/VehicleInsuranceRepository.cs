using api.Interfaces.Repository;
using api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Repository
{
    public class VehicleInsuranceRepository : IVehicleInsuranceRepository
    {
        private readonly PrnprojectContext _context;

        public VehicleInsuranceRepository(PrnprojectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VehicleInsurance>> GetAllAsync()
        {
            return await _context.VehicleInsurances.Where(v => !v.IsDeleted).ToListAsync();
        }

        public async Task<VehicleInsurance?> GetByIdAsync(int id)
        {
            return await _context.VehicleInsurances.FindAsync(id);
        }

        public async Task<IEnumerable<VehicleInsurance>> GetByVehicleIdAsync(int vehicleId)
        {
            return await _context.VehicleInsurances
                .Where(x => x.VehicleId == vehicleId && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<VehicleInsurance>> SearchAsync(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return Enumerable.Empty<VehicleInsurance>();

            var list = await _context.VehicleInsurances.Where(v => !v.IsDeleted).ToListAsync();

            if (DateTimeOffset.TryParse(q, out var dt))
                return list.Where(x => x.StartDate.Date <= dt.Date && x.EndDate.Date >= dt.Date);

            return list.Where(x =>
                (!string.IsNullOrEmpty(x.InsuranceProvider) && x.InsuranceProvider.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(x.PolicyNumber) && x.PolicyNumber.Contains(q, StringComparison.OrdinalIgnoreCase))
            );
        }

        public async Task AddAsync(VehicleInsurance entity)
        {
            await _context.VehicleInsurances.AddAsync(entity);
        }

        public void Update(VehicleInsurance entity)
        {
            _context.VehicleInsurances.Update(entity);
        }

        public void Remove(VehicleInsurance entity)
        {
            _context.VehicleInsurances.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}