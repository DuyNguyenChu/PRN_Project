using System.Globalization;
using api.DTParameters;
using api.Extensions;
using api.Helpers;
using api.Interface.Repository;
using api.Models;
using api.ViewModel;
using Microsoft.EntityFrameworkCore;
using static api.Extensions.ApiCodeConstants;

namespace api.Repositories
{
    public class TripExpenseRepository : RepositoryBase<TripExpense, int>, ITripExpenseRepository
    {
        private readonly PrnprojectContext _context;
        public TripExpenseRepository(PrnprojectContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
        }
        public async Task<PagingData<TripExpenseAggregate>> GetPagedAsync(TripExpenseFilter filter)
        {
            var keyword = filter.Keyword;

            var query = from a in _context.TripExpenses
                        join h in _context.ExpenseTypes on a.ExpenseTypeId equals h.Id
                        join f in _context.Trips on a.TripId equals f.Id into groupAF
                        from f in groupAF.DefaultIfEmpty()
                        join b in _context.Drivers on a.DriverId equals b.Id
                        let user = b.Users.FirstOrDefault(u => !u.IsDeleted)
                        join d in _context.Vehicles on a.VehicleId equals d.Id
                        join g in _context.VehicleModels on d.VehicleModelId equals g.Id
                        join vb in _context.VehicleBranches on d.VehicleBranchId equals vb.Id
                        where !a.IsDeleted && !b.IsDeleted && !d.IsDeleted && !g.IsDeleted && !h.IsDeleted && !vb.IsDeleted
                            && (f == null || !f.IsDeleted)
                        select new TripExpenseAggregate
                        {
                            Id = a.Id,
                            ApprovalBy = a.ApprovedBy,
                            ApprovalDate = a.ApprovedDate,
                            //ApprovalUser = e == null ? null : e.FirstName + " " + e.LastName,
                            DriverId = a.DriverId,
                            DriverName = user.FirstName + " " + user.LastName,
                            Notes = a.Notes,
                            RejectReason = a.RejectReason,
                            Status = a.Status,
                            //TripCode = f == null ? null : f.TripCode,
                            TripId = a.TripId,
                            VehicleId = a.VehicleId,
                            //VehicleLicensePlate = d.LicensePlate,
                            CreatedDate = a.CreatedDate,
                            Amount = a.Amount,
                            ExpenseDate = a.OccurenceDate,
                            ExpenseTypeId = a.ExpenseTypeId,
                            ExpenseTypeName = h.Name,
                            VehicleBrandName = vb.Name,
                            VehicleModelName = g.Name,
                        };

            if (filter.DriverIds.Any())
            {
                query = query
                    .Where(x => filter.DriverIds.Contains(x.DriverId));
            }

            if (filter.TripIds.Any())
            {
                query = query
                    .Where(x => x.TripId != null && filter.TripIds.Contains(x.TripId.Value));
            }

            //if (filter.TripCodes.Any())
            //{
            //    query = query
            //        .Where(x => x.TripCode != null && filter.TripCodes.Contains(x.TripCode));
            //}

            var totalRecord = await query.CountAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();

                query = query
                    .Where(x =>
                    (x.ApprovalDate != null && x.ApprovalDate.Value.ToVietnameseDateTimeOffset().Contains(keyword)) ||
                    EF.Functions.Collate(x.DriverName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    (x.Notes != null && EF.Functions.Collate(x.Notes.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                    (x.RejectReason != null && EF.Functions.Collate(x.RejectReason.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                    x.Amount.ToString().Contains(keyword) ||
                    EF.Functions.Collate(x.ExpenseTypeName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    x.ExpenseDate.ToVietnameseDateTimeOffset().Contains(keyword) ||
                    //(x.TripCode != null && x.TripCode.Contains(keyword)) ||
                    //EF.Functions.Collate(x.VehicleLicensePlate.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    EF.Functions.Collate(x.VehicleBrandName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    EF.Functions.Collate(x.VehicleModelName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    x.CreatedDate.ToVietnameseDateTimeOffset().Contains(keyword)
                );
            }

            if (!string.IsNullOrEmpty(filter.PeriodTime))
            {
                filter.PeriodTime = filter.PeriodTime.Replace(" ", "");
                if (filter.PeriodTime.Contains("-"))
                {
                    var dates = filter.PeriodTime.Split("-");
                    var startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                    query = query.Where(c => c.ExpenseDate >= startDate && c.ExpenseDate <= endDate);
                }
                else
                {
                    var date = DateTime.ParseExact(filter.PeriodTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    query = query.Where(c => c.ExpenseDate.Date == date.Date);
                }
            }

            if (filter.StatusIds.Any())
            {
                query = query
                    .Where(x => filter.StatusIds.Contains(x.Status));
            }

            if (filter.ExpenseTypeIds.Any())
            {
                query = query
                    .Where(x => filter.ExpenseTypeIds.Contains(x.ExpenseTypeId));
            }

            if (filter.VehicleIds.Any())
            {
                query = query
                    .Where(x => filter.VehicleIds.Contains(x.VehicleId));
            }

            var totalFiltered = await query.CountAsync();

            if (string.IsNullOrEmpty(filter.OrderBy))
            {
                query = query
                    .OrderByDescending(x => x.ExpenseDate);
            }
            else
            {
                query = query.OrderByDynamic(filter.OrderBy, filter.SortType == "asc" ? LinqExtensions.Order.Asc : LinqExtensions.Order.Desc);
            }

            var pagedData = new PagingData<TripExpenseAggregate>
            {
                CurrentPage = filter.PageIndex,
                PageSize = filter.PageSize,
                DataSource = await query.Skip((filter.PageIndex - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(),
                Total = totalRecord,
                TotalFiltered = totalFiltered
            };

            return pagedData;
        }

        public async Task<DTResult<TripExpenseAggregate>> GetPagedAsync(TripExpenseDTParameters parameters)
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
                orderCriteria = "Id";
                orderAscendingDirection = true;
            }


            var query = from a in _context.TripExpenses
                        join h in _context.ExpenseTypes on a.ExpenseTypeId equals h.Id
                        join f in _context.Trips on a.TripId equals f.Id into groupAF
                        from f in groupAF.DefaultIfEmpty()
                        join b in _context.Drivers on a.DriverId equals b.Id
                        let user = b.Users.FirstOrDefault(u => !u.IsDeleted)
                        join d in _context.Vehicles on a.VehicleId equals d.Id
                        join g in _context.VehicleModels on d.VehicleModelId equals g.Id
                        join vb in _context.VehicleBranches on d.VehicleBranchId equals vb.Id
                        where !a.IsDeleted && !b.IsDeleted && !d.IsDeleted && !g.IsDeleted && !h.IsDeleted
                            && (f == null || !f.IsDeleted)
                        select new TripExpenseAggregate
                        {
                            Id = a.Id,
                            ApprovalBy = a.ApprovedBy,
                            ApprovalDate = a.ApprovedDate,
                            //ApprovalUser = e == null ? null : e.FirstName + " " + e.LastName,
                            DriverId = a.DriverId,
                            DriverName = user.FirstName + " " + user.LastName,
                            Notes = a.Notes,
                            RejectReason = a.RejectReason,
                            Status = a.Status,
                            //TripCode = f == null ? null : f.TripCode,
                            TripId = a.TripId,
                            VehicleId = a.VehicleId,
                            //VehicleLicensePlate = d.LicensePlate,
                            VehicleModelName = g.Name,
                            VehicleBrandName = vb.Name,
                            CreatedDate = a.CreatedDate,
                            Amount = a.Amount,
                            ExpenseDate = a.OccurenceDate,
                            ExpenseTypeId = a.ExpenseTypeId,
                            ExpenseTypeName = h.Name,
                        };

            var totalRecord = await query.CountAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query
                    .Where(x => 
                    (x.ApprovalDate != null && x.ApprovalDate.Value.ToVietnameseDateTimeOffset().Contains(keyword)) ||
                    EF.Functions.Collate(x.DriverName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    (x.Notes != null && EF.Functions.Collate(x.Notes.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                    (x.RejectReason != null && EF.Functions.Collate(x.RejectReason.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                    x.Amount.ToString().Contains(keyword) ||
                    EF.Functions.Collate(x.ExpenseTypeName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    x.ExpenseDate.ToVietnameseDateTimeOffset().Contains(keyword) ||
                    //(x.TripCode != null && x.TripCode.Contains(keyword)) ||
                    //EF.Functions.Collate(x.VehicleLicensePlate.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    EF.Functions.Collate(x.VehicleBrandName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    EF.Functions.Collate(x.VehicleModelName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                    x.CreatedDate.ToVietnameseDateTimeOffset().Contains(keyword)
                );
            }

            foreach (var column in parameters.Columns)
            {
                var search = column.Search.Value;
                if (!search.Contains("/"))
                {
                    search = column.Search.Value.ToLower();
                }
                if (string.IsNullOrEmpty(search)) continue;
                switch (column.Data)
                {
                    //case "tripCode":
                    //    query = query
                    //        .Where(r => r.TripCode != null && EF.Functions.Collate(r.TripCode.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General))); ;
                    //    break;
                    case "expenseDate":
                        if (search.Contains(" - "))
                        {
                            var dates = search.Split(" - ");
                            var startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            var endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                            query = query.Where(c => c.ExpenseDate >= startDate && c.ExpenseDate <= endDate);
                        }
                        else
                        {
                            var date = DateTime.ParseExact(search, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            query = query.Where(c => c.ExpenseDate.Date == date.Date);
                        }
                        break;

                    case "createdDate":
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

                    case "approvalDate":
                        if (search.Contains(" - "))
                        {
                            var dates = search.Split(" - ");
                            var startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            var endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                            query = query.Where(c => c.ApprovalDate.HasValue && c.ApprovalDate >= startDate && c.ApprovalDate <= endDate);
                        }
                        else
                        {
                            var date = DateTime.ParseExact(search, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            query = query.Where(c => c.ApprovalDate.HasValue && c.ApprovalDate.Value.Date == date.Date);
                        }
                        break;
                }
            }

            if (parameters.StatusIds.Any())
            {
                query = query
                    .Where(x => parameters.StatusIds.Contains(x.Status));
            }

            if (parameters.ExpenseTypeIds.Any())
            {
                query = query
                    .Where(x => parameters.ExpenseTypeIds.Contains(x.ExpenseTypeId));
            }

            if (parameters.VehicleIds.Any())
            {
                query = query
                    .Where(x => parameters.VehicleIds.Contains(x.VehicleId));
            }

            if (parameters.DriverIds.Any())
            {
                query = query
                    .Where(x => parameters.DriverIds.Contains(x.DriverId));
            }

            if (parameters.TripIds.Any())
            {
                query = query
                    .Where(x => x.TripId != null && parameters.TripIds.Contains(x.TripId.Value));
            }

            query = orderAscendingDirection ? query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) : query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);

            var records = await query.Skip(parameters.Start).Take(parameters.Length).ToListAsync();
            records.ForEach(item =>
            {
                var status = CommonConstants.ApprovalStatuses.FirstOrDefault(x => x.Id == item.Status);
                item.StatusColor = status?.Color ?? string.Empty;
                item.StatusName = status?.Name ?? string.Empty;
            });

            var data = new DTResult<TripExpenseAggregate>
            {
                draw = parameters.Draw,
                data = records,
                recordsFiltered = await query.CountAsync(),
                recordsTotal = totalRecord
            };

            return data;

        }
    }
}
