using api.Models;
using api.Interface.Repository;
using api.Interface;
using api.ViewModel;
using api.DTParameters;
using api.Extensions;
using api.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace api.Repositories
{
    public class MaintenanceRecordRepository : RepositoryBase<MaintenanceRecord, int>, IMaintenanceRecordRepository
    {
        private readonly PrnprojectContext _context;
        public MaintenanceRecordRepository(PrnprojectContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
        }

        public async Task<DTResult<MaintenanceRecordAggregate>> GetPagedAsync(MaintenanceRecordDTParameters parameters)
        {
            var keyword = parameters.Search?.Value;
            var orderCriteria = string.Empty;
            var orderAscendingDirection = true;
            if (parameters.Order != null && parameters.Order.Any())
            {
                orderCriteria = parameters.Columns[parameters.Order[0].Column].Data;
                orderAscendingDirection = parameters.Order[0].Dir.ToString().ToLower() == "asc";
            }
            else
            {
                orderCriteria = "StartTime";
                orderAscendingDirection = false;
            }

            var query = from a in _context.MaintenanceRecords
                        join f in _context.Trips on a.TripId equals f.Id into groupAF
                        from f in groupAF.DefaultIfEmpty()
                        join b in _context.Drivers on a.DriverId equals b.Id

                        // === SỬA LỖI 1: CHUYỂN SANG LEFT JOIN VỚI BẢNG USER ===
                        join c in _context.Users on b.Id equals c.DriverId into groupBC
                        from c in groupBC.DefaultIfEmpty() // 'c' (user) bây giờ có thể null

                        join d in _context.Vehicles on a.VehicleId equals d.Id
                        join g in _context.VehicleModels on d.VehicleModelId equals g.Id
                        join e in _context.Users on a.ApprovedBy equals e.Id into groupAE
                        from e in groupAE.DefaultIfEmpty()

                            // Thêm kiểm tra 'c' null
                        where !a.IsDeleted && !b.IsDeleted && (c == null || !c.IsDeleted) && !d.IsDeleted && !g.IsDeleted
                        && (e == null || !e.IsDeleted) && (f == null || !f.IsDeleted)

                        select new MaintenanceRecordAggregate
                        {
                            Id = a.Id,
                            ApprovedBy = a.ApprovedBy,
                            ApprovedDate = a.ApprovedDate,
                            ApprovedByName = e == null ? null : e.FirstName + " " + e.LastName,
                            DriverId = a.DriverId,
                            // Xử lý 'c' null
                            DriverName = c == null ? "[Không có User]" : c.FirstName + " " + c.LastName,
                            EndTime = a.EndTime,
                            Notes = a.Notes,
                            Odometer = a.Odometer,
                            RejectReason = a.RejectReason,
                            ServiceCost = a.ServiceCost,
                            ServiceProvider = a.ServiceProvider,
                            ServiceType = a.ServiceType,
                            StartTime = a.StartTime,
                            Status = a.Status,
                            TripCode = f == null ? null : f.Description,
                            TripId = a.TripId,
                            VehicleId = a.VehicleId,
                            VehicleRegistrationNumber = d.RegistrationNumber,
                            VehicleModelName = g.Name,
                            CreatedDate = a.CreatedDate,
                        };

            var totalRecord = await query.CountAsync();

            // === SỬA LỖI 2: SỬA LOGIC TÌM KIẾM ===
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query
                .Where(x =>
                    x.StartTime.ToVietnameseDateTimeOffset().Contains(keyword) ||
                    // Dùng EF.Functions.Collate
                    EF.Functions.Collate(x.DriverName, SQLParams.Latin_General).Contains(keyword) ||
                    EF.Functions.Collate(x.VehicleRegistrationNumber, SQLParams.Latin_General).Contains(keyword) ||
                    EF.Functions.Collate(x.VehicleModelName, SQLParams.Latin_General).Contains(keyword) ||
                    (x.TripCode != null && EF.Functions.Collate(x.TripCode, SQLParams.Latin_General).Contains(keyword)) ||
                    EF.Functions.Collate(x.ServiceProvider, SQLParams.Latin_General).Contains(keyword)

                // XÓA 2 DÒNG GÂY LỖI .ToString()
                // || x.Odometer.ToString().Contains(keyword) ||
                // (x.ServiceCost.HasValue && x.ServiceCost.Value.ToString().Contains(keyword))
                );
            }

            // Lọc theo cột (Sửa logic .ToLower())
            if (parameters.Columns != null)
            {
                foreach (var column in parameters.Columns)
                {
                    var search = column.Search?.Value;
                    if (string.IsNullOrEmpty(search)) continue;
                    switch (column.Data)
                    {
                        case "startTime":
                            if (search.Contains(" - "))
                            {
                                var dates = search.Split(" - ");
                                var startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                var endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                query = query.Where(c => c.StartTime >= startDate && c.StartTime <= endDate);
                            }
                            else
                            {
                                var date = DateTime.ParseExact(search, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                query = query.Where(c => c.StartTime.Date == date.Date);
                            }
                            break;
                        case "tripCode":
                            query = query.Where(r => r.TripCode != null &&
                                EF.Functions.Collate(r.TripCode, SQLParams.Latin_General).Contains(search));
                            break;
                        case "serviceProvider":
                            query = query.Where(r =>
                                EF.Functions.Collate(r.ServiceProvider, SQLParams.Latin_General).Contains(search));
                            break;
                    }
                }
            }

            // Lọc nâng cao (Không đổi)
            if (parameters.StatusIds.Any())
                query = query.Where(x => parameters.StatusIds.Contains(x.Status));
            // ... (các filter khác không đổi) ...

            query = orderAscendingDirection ? query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) : query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);

            var records = await query.Skip(parameters.Start).Take(parameters.Length).ToListAsync();

            // Gán tên hiển thị (Không đổi)
            records.ForEach(item =>
            {
                var status = CommonConstants.ApprovalStatuses.FirstOrDefault(x => x.Id == item.Status);
                item.StatusColor = status?.Color ?? string.Empty;
                item.StatusName = status?.Name ?? string.Empty;
                item.ServiceTypeName = CommonConstants.ServiceType.GetValueOrDefault(item.ServiceType) ?? string.Empty;
            });

            return new DTResult<MaintenanceRecordAggregate>
            {
                draw = parameters.Draw,
                data = records,
                recordsFiltered = await query.CountAsync(), // Đếm sau khi lọc
                recordsTotal = totalRecord
            };
        }
    }
}