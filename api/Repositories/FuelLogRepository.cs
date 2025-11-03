using api.Dtos.FuelLog;
using api.DTParameters;
using api.Extensions; 
using api.Helpers;
using api.Interface; 
using api.Interface.Repository;
using api.Models;
using api.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace api.Repositories
{
    public class FuelLogRepository : RepositoryBase<FuelLog, int>, IFuelLogRepository
    {
        private readonly PrnprojectContext _context;
        public FuelLogRepository(PrnprojectContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
        }

        // Tạm thời chưa triển khai phương thức này
        public Task<PagingData<FuelLogAggregate>> GetFuelLogsAsync(FuelLogSearchQuery query)
        {
            throw new NotImplementedException();
        }


        public async Task<DTResult<FuelLogAggregate>> GetPagedAsync(FuelLogDTParameters parameters)
        {
            var keyword = parameters.Search?.Value;
            var orderCriteria = string.Empty;
            var orderAscendingDirection = true;

            if (parameters.Order != null && parameters.Order.Any())
            {
                // Đảm bảo rằng tên cột khớp với tên thuộc tính trong FuelLogAggregate
                orderCriteria = parameters.Columns[parameters.Order[0].Column].Data;
                orderAscendingDirection = parameters.Order[0].Dir.ToString().ToLower() == "asc";
            }
            else
            {
                orderCriteria = "CreatedDate"; // Sắp xếp mặc định theo ngày tạo
                orderAscendingDirection = false;
            }

            var query = from fl in _context.FuelLogs
                        join v in _context.Vehicles on fl.VehicleId equals v.Id
                        join vm in _context.VehicleModels on v.VehicleModelId equals vm.Id
                        join d in _context.Drivers on fl.DriverId equals d.Id
                        join du in _context.Users on d.Id equals du.DriverId // Join User thông qua DriverId trên User
                        join t in _context.Trips on fl.TripId equals t.Id into tg
                        from t in tg.DefaultIfEmpty()
                        join u in _context.Users on fl.ApprovedBy equals u.Id into ug 
                        from u in ug.DefaultIfEmpty()
                        where !fl.IsDeleted &&
                              !v.IsDeleted &&
                              !d.IsDeleted &&
                              !du.IsDeleted &&
                              !vm.IsDeleted &&
                              (t == null || !t.IsDeleted) &&
                              (u == null || !u.IsDeleted)
                        select new FuelLogAggregate
                        {
                            Id = fl.Id,
                            VehicleId = fl.VehicleId,
                            VehicleModelName = vm.Name,
                            VehicleRegistrationNumber = v.RegistrationNumber, // Đã đổi
                            DriverId = fl.DriverId,
                            DriverName = du.FirstName + " " + du.LastName,
                            TripId = fl.TripId,
                            TripCode = t != null ? t.Description : null,
                            Odometer = fl.Odometer,
                            FuelType = fl.FuelType,
                            UnitPrice = fl.UnitPrice,
                            Quantity = fl.Quantity,
                            TotalCost = fl.TotalCost,
                            GasStation = fl.GasStation, 
                            Notes = fl.Notes,
                            Status = fl.Status,
                            ApprovedBy = fl.ApprovedBy, 
                            ApprovedByName = u != null ? u.FirstName + " " + u.LastName : null, 
                            ApprovedDate = fl.ApprovedDate,
                            RejectReason = fl.RejectReason,
                            CreatedDate = fl.CreatedDate 
                        };

            var totalRecord = await query.CountAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query
                    .Where(x => x.FuelType.ToLower().Contains(keyword) ||
                          x.CreatedDate.DateTime.ToVietnameseDateTime().Contains(keyword) ||
                          x.VehicleModelName.ToLower().Contains(keyword) ||
                          x.VehicleRegistrationNumber.ToLower().Contains(keyword) ||
                          x.DriverName.ToLower().Contains(keyword) ||
                          (x.TripCode != null && x.TripCode.Contains(keyword)) ||
                          x.GasStation.ToLower().Contains(keyword) ||
                          x.TotalCost.ToString().Contains(keyword) ||
                          x.Odometer.ToString().Contains(keyword)
                    );
            }

            // Lọc theo cột (nếu có)
            if (parameters.Columns != null)
            {
                foreach (var column in parameters.Columns)
                {
                    var search = column.Search?.Value;
                    if (string.IsNullOrEmpty(search)) continue;

                    switch (column.Data)
                    {
                        case "tripCode":
                            query = query.Where(r => r.TripCode != null && r.TripCode.ToLower().Contains(search.ToLower()));
                            break;
                        case "gasStation":
                            query = query.Where(r => r.GasStation.ToLower().Contains(search.ToLower()));
                            break;
                        case "createdDate": // Đã đổi từ refuelingDate
                            if (search.Contains(" - "))
                            {
                                var dates = search.Split(" - ");
                                var startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                var endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                                query = query.Where(c => c.CreatedDate >= startDate && c.CreatedDate <= endDate);
                            }
                            else
                            {
                                var date = DateTime.ParseExact(search, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                query = query.Where(c => c.CreatedDate.Date == date.Date);
                            }
                            break;
                    }
                }
            }

            // Lọc nâng cao
            if (parameters.VehicleIds.Any())
                query = query.Where(x => parameters.VehicleIds.Contains(x.VehicleId));
            if (parameters.DriverIds.Any())
                query = query.Where(x => parameters.DriverIds.Contains(x.DriverId));
            if (parameters.FuelTypes.Any())
                query = query.Where(x => parameters.FuelTypes.Contains(x.FuelType));
            if (parameters.StatusIds.Any())
                query = query.Where(x => parameters.StatusIds.Contains(x.Status));
            if (parameters.TripIds.Any())
                query = query.Where(x => x.TripId.HasValue && parameters.TripIds.Contains(x.TripId.Value));

            // Sắp xếp
            query = orderAscendingDirection ? query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) : query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);

            var records = await query
                .Skip(parameters.Start)
                .Take(parameters.Length)
                .ToListAsync();

            // Gán tên hiển thị
            foreach (var item in records)
            {
                item.FuelTypeName = CommonConstants.GetFuelTypeName(item.FuelType);
                var status = CommonConstants.ApprovalStatuses.FirstOrDefault(xx => xx.Id == item.Status);
                item.StatusColor = status?.Color ?? string.Empty;
                item.StatusName = status?.Name ?? string.Empty;
            }

            return new DTResult<FuelLogAggregate>
            {
                draw = parameters.Draw,
                data = records,
                recordsFiltered = await query.CountAsync(),
                recordsTotal = totalRecord
            };
        }
    }
}