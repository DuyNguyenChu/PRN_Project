using api.DTParameters;
using api.Extensions;
using api.Interface.Repository;
using api.Models;
using api.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Repositories
{
    public class DriverViolationRepository : RepositoryBase<DriverViolation, int>, IDriverViolationRepository
    {
        private readonly PrnprojectContext _context;
        public DriverViolationRepository(PrnprojectContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
        }
        public async Task<DTResult<DriverViolationAggregate>> GetPagedAsync(DriverViolationDTParameters parameters)
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

            var query = from a in _context.DriverViolations
                        join h in _context.ViolationTypes on a.ViolationTypeId equals h.Id
                        join b in _context.Drivers on a.DriverId equals b.Id
                        join c in _context.Users on b.Users.FirstOrDefault().Id equals c.Id
                        join d in _context.Vehicles on a.VehicleId equals d.Id
                        join g in _context.VehicleModels on d.VehicleModelId equals g.Id
                        where !a.IsDeleted && !b.IsDeleted && !c.IsDeleted && !d.IsDeleted && !g.IsDeleted && !h.IsDeleted
                        select new DriverViolationAggregate
                        {
                            Id = a.Id,
                            DriverId = a.DriverId,
                            DriverName = c.FirstName + " " + c.LastName,
                            //TripCode = f == null ? null : f.TripCode,
                            //TripId = a.TripId,
                            VehicleId = a.VehicleId,
                            VehicleName = g.Name,
                            CreatedDate = a.CreatedDate,
                            Description = a.Description,
                            Location = a.ViolationLocation,
                            PenaltyAmount = a.PenaltyAmount,
                            //PointsDeducted = a.PointsDeducted,
                            ReportedBy = a.ReportedBy,
                            ViolationDate = a.ViolationDate,
                            ViolationTypeId = a.ViolationTypeId,
                            ViolationTypeName = h.Name
                        };

            var totalRecord = await query.CountAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query
                    .Where(x => x.ViolationDate.ToVietnameseDateTimeOffset().Contains(keyword) ||
                        EF.Functions.Collate(x.DriverName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                        (x.Description != null && EF.Functions.Collate(x.Description.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        x.PenaltyAmount.ToString().Contains(keyword) ||
                        EF.Functions.Collate(x.ViolationTypeName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                        EF.Functions.Collate(x.Location.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                         EF.Functions.Collate(x.ReportedBy.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                        //(x.TripCode != null && x.TripCode.Contains(keyword)) ||
                        EF.Functions.Collate(x.VehicleName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
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
                    case "reportBy":
                        query = query
                            .Where(x => EF.Functions.Collate(x.ReportedBy.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(search, SQLParams.Latin_General)));
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

                    case "violationDate":
                        if (search.Contains(" - "))
                        {
                            var dates = search.Split(" - ");
                            var startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            var endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                            query = query.Where(c => c.ViolationDate >= startDate && c.ViolationDate <= endDate);
                        }
                        else
                        {
                            var date = DateTime.ParseExact(search, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            query = query.Where(c => c.ViolationDate.Date == date.Date);
                        }
                        break;
                }
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

            //if (parameters.TripIds.Any())
            //{
            //    query = query
            //        .Where(x => x.TripId != null && parameters.TripIds.Contains(x.TripId.Value));
            //}

            if (parameters.VehicleTypeIds.Any())
            {
                query = query
                    .Where(x => parameters.VehicleTypeIds.Contains(x.ViolationTypeId));
            }

            query = orderAscendingDirection ? query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) : query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);

            var records = await query.Skip(parameters.Start).Take(parameters.Length).ToListAsync();

            var data = new DTResult<DriverViolationAggregate>
            {
                draw = parameters.Draw,
                data = records,
                recordsFiltered = await query.CountAsync(),
                recordsTotal = totalRecord
            };

            return data;
        }

        public async Task<PagingData<DriverViolationAggregate>> GetPagedAsync(DriverViolationFilter filter)
        {
            var keyword = filter.Keyword;

            var query = from a in _context.DriverViolations
                        join h in _context.ViolationTypes on a.ViolationTypeId equals h.Id
                        join b in _context.Drivers on a.DriverId equals b.Id
                        join c in _context.Users on b.Users.FirstOrDefault().Id equals c.Id
                        join d in _context.Vehicles on a.VehicleId equals d.Id
                        join g in _context.VehicleModels on d.VehicleModelId equals g.Id
                        where !a.IsDeleted && !b.IsDeleted && !c.IsDeleted && !d.IsDeleted && !g.IsDeleted && !h.IsDeleted
                        select new DriverViolationAggregate
                        {
                            Id = a.Id,
                            DriverId = a.DriverId,
                            DriverName = c.FirstName + " " + c.LastName,
                            //TripCode = f == null ? null : f.TripCode,
                            //TripId = a.TripId,
                            VehicleId = a.VehicleId,
                            VehicleName = g.Name,
                            CreatedDate = a.CreatedDate,
                            Description = a.Description,
                            Location = a.ViolationLocation,
                            PenaltyAmount = a.PenaltyAmount,
                            //PointsDeducted = a.PointsDeducted,
                            ReportedBy = a.ReportedBy,
                            ViolationDate = a.ViolationDate,
                            ViolationTypeId = a.ViolationTypeId,
                            ViolationTypeName = h.Name
                        };

            var totalRecord = await query.CountAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query
                    .Where(x => x.ViolationDate.ToVietnameseDateTimeOffset().Contains(keyword) ||
                        EF.Functions.Collate(x.DriverName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                        (x.Description != null && EF.Functions.Collate(x.Description.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General))) ||
                        x.PenaltyAmount.ToString().Contains(keyword) ||
                        EF.Functions.Collate(x.ViolationTypeName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                        EF.Functions.Collate(x.Location.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                         EF.Functions.Collate(x.ReportedBy.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                        //(x.TripCode != null && x.TripCode.Contains(keyword)) ||
                        EF.Functions.Collate(x.VehicleName.ToLower(), SQLParams.Latin_General).Contains(EF.Functions.Collate(keyword, SQLParams.Latin_General)) ||
                        x.CreatedDate.ToVietnameseDateTimeOffset().Contains(keyword)
                    );
            }

            if (filter.VehicleIds.Any())
            {
                query = query
                    .Where(x => filter.VehicleIds.Contains(x.VehicleId));
            }

            if (filter.DriverIds.Any())
            {
                query = query
                    .Where(x => filter.DriverIds.Contains(x.DriverId));
            }

            //if (filter.TripIds.Any())
            //{
            //    query = query
            //        .Where(x => x.TripId != null && filter.TripIds.Contains(x.TripId.Value));
            //}

            //if (filter.TripCodes.Any())
            //{
            //    query = query
            //        .Where(x => x.TripCode != null && filter.TripCodes.Contains(x.TripCode));
            //}

            if (filter.ViolationTypeIds.Any())
            {
                query = query
                    .Where(x => filter.ViolationTypeIds.Contains(x.ViolationTypeId));
            }

            if (string.IsNullOrEmpty(filter.OrderBy))
            {
                query = query
                    .OrderByDescending(x => x.ViolationDate);
            }
            else
            {
                query = query.OrderByDynamic(filter.OrderBy, filter.SortType == "asc" ? LinqExtensions.Order.Asc : LinqExtensions.Order.Desc);
            }

            var pagedData = new PagingData<DriverViolationAggregate>
            {
                CurrentPage = filter.PageIndex,
                PageSize = filter.PageSize,
                DataSource = await query.Skip((filter.PageIndex - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(),
                Total = totalRecord,
                TotalFiltered = await query.CountAsync()
            };

            return pagedData;
        }

    }
}
