// ...existing code...
using api.Interfaces;
using api.Interfaces.Repository;
using api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Repository
{
    public class VehicleAccidentRepository : IVehicleAccidentRepository
    {
        private readonly PrnprojectContext _context;

        public VehicleAccidentRepository(PrnprojectContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VehicleAccident>> GetAllAsync()
        {
            return await _context.VehicleAccidents.Where(v => !v.IsDeleted).ToListAsync();
        }

        public async Task<VehicleAccident?> GetByIdAsync(int id)
        {
            return await _context.VehicleAccidents.FindAsync(id);
        }

        public async Task<IEnumerable<VehicleAccident>> SearchAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Enumerable.Empty<VehicleAccident>();

            // Lấy các bản ghi chưa xoá vào bộ nhớ trước rồi lọc (đơn giản và tránh lỗi translate SQL cho .ToString/Contains)
            var list = await _context.VehicleAccidents
                .Where(v => !v.IsDeleted)
                .ToListAsync();

            // Nếu input có thể parse thành ngày, so sánh theo ngày (ignore time)
            if (DateTime.TryParse(name, out var parsedDate))
            {
                return list.Where(v => v.AccidentDate.Date == parsedDate.Date);
            }

            // Ngược lại tìm theo Location / Description hoặc theo chuỗi ngày đã format
            return list.Where(v =>
                (!string.IsNullOrEmpty(v.Location) && v.Location.Contains(name, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(v.Description) && v.Description.Contains(name, StringComparison.OrdinalIgnoreCase)) ||
                v.AccidentDate.ToString("yyyy-MM-dd").Contains(name, StringComparison.OrdinalIgnoreCase)
            );
        }

        public async Task AddAsync(VehicleAccident vehicle)
        {
            await _context.VehicleAccidents.AddAsync(vehicle);
        }

        public void Update(VehicleAccident vehicle)
        {
            _context.VehicleAccidents.Update(vehicle);
        }

        public void Remove(VehicleAccident vehicle)
        {
            _context.VehicleAccidents.Remove(vehicle);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
